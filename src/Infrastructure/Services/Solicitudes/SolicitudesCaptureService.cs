using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Application.Features.Solicitudes;
using Domain.Entities.Solicitudes;
using Infrastructure.Persistence.Solicitudes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Services.Solicitudes;

internal sealed class SolicitudesCaptureService(
    SolicitudesDbContext dbContext,
    IConfiguration configuration) : ISolicitudesCaptureService
{
    private static readonly string[] KnownFlags =
    [
        "SOLO_CPMS",
        "BUSCAR_EXISTENCIA_EN_CLUES",
        "APLICAR_ENCUESTAS",
        "APLICAR_EQUIVALENCIAS",
        "CLUES_EXISTENCIAS_ALLOWLIST",
        "IMPORT_LIMIT_TO_KIT",
        "EDIT_CPMS"
    ];
    private static readonly HashSet<string> BooleanFlags = new(
        ["SOLO_CPMS", "BUSCAR_EXISTENCIA_EN_CLUES", "APLICAR_ENCUESTAS", "APLICAR_EQUIVALENCIAS", "IMPORT_LIMIT_TO_KIT", "EDIT_CPMS"],
        StringComparer.Ordinal);

    public async Task<IReadOnlyList<UnidadSolicitudDto>> GetUnidadesAsync(
        string? query,
        string? nivel,
        CancellationToken cancellationToken)
    {
        var filtered = BuildUnidadesQuery(query, nivel);
        return await filtered
            .OrderBy(item => item.NombreMunicipio)
            .ThenBy(item => item.NombreLocalidad)
            .ThenBy(item => item.NombreDeUnidad)
            .Select(item => new UnidadSolicitudDto(
                item.Id ?? 0,
                item.Cluessa,
                item.Cluesimb,
                item.NombreMunicipio,
                item.NombreLocalidad,
                item.NombreTipologia,
                item.EsSegundoNivel,
                item.NombreDeUnidad,
                item.TipoUnidad,
                item.AliasSas,
                item.Direccion,
                item.Latitud,
                item.Longitud,
                item.EstratoUnidad,
                item.NivelAtencion))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UnidadExistenteDto>> GetUnidadesPrimerNivelAsync(
        string? query,
        CancellationToken cancellationToken)
    {
        var filtered = BuildUnidadesQuery(query, "PRIMER_NIVEL")
            .Where(item => item.TipoUnidad == "CENTRO DE SALUD");

        return await filtered
            .OrderBy(item => item.NombreMunicipio)
            .ThenBy(item => item.Cluesimb)
            .Select(item => MapUnidadExistente(item, "CENTROS DE SALUD"))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UnidadExistenteDto>> GetUnidadesTodosLosNivelesAsync(
        string? query,
        CancellationToken cancellationToken) =>
        await BuildUnidadesQuery(query, null)
            .OrderBy(item => item.NombreMunicipio)
            .ThenBy(item => item.Cluesimb)
            .Select(item => MapUnidadExistente(item, item.TipoUnidad ?? string.Empty))
            .ToListAsync(cancellationToken);

    public async Task<BuscarArticulosResponse> BuscarArticulosAsync(string query, CancellationToken cancellationToken)
    {
        var normalized = query.Trim();
        var filtered = dbContext.Articulos.AsNoTracking().Where(item =>
            EF.Functions.ILike(item.Clave ?? string.Empty, $"%{normalized}%") ||
            EF.Functions.ILike(item.Descripcion ?? string.Empty, $"%{normalized}%"));

        var total = await filtered.CountAsync(cancellationToken);
        var results = await filtered
            .OrderBy(item => item.Clave == null)
            .ThenBy(item => item.Clave)
            .Take(12)
            .Select(item => new ArticuloSolicitudDto(
                item.Clave ?? string.Empty,
                item.Descripcion ?? string.Empty,
                item.Presentacion ?? string.Empty))
            .ToListAsync(cancellationToken);

        return new BuscarArticulosResponse(results, total);
    }

    public async Task<BuscarArticulosResponse> GetArticulosAllAsync(CancellationToken cancellationToken)
    {
        var results = await dbContext.Articulos.AsNoTracking()
            .OrderBy(item => item.Clave == null)
            .ThenBy(item => item.Clave)
            .Select(item => new ArticuloSolicitudDto(
                item.Clave ?? string.Empty,
                item.Descripcion ?? string.Empty,
                item.Presentacion ?? string.Empty))
            .ToListAsync(cancellationToken);

        return new BuscarArticulosResponse(results, results.Count);
    }

    public async Task<BuscarArticulosResponse> GetArticulosByCluesimbCpmAsync(
        string cluesimb,
        CancellationToken cancellationToken)
    {
        var normalizedCluesimb = NormalizeKey(cluesimb);
        if (normalizedCluesimb == null)
        {
            throw new ArgumentException("cluesimb es requerido.");
        }

        var results = await (
            from article in dbContext.Articulos.AsNoTracking()
            join cpm in dbContext.VUnidadCpms.AsNoTracking()
                on article.Clave equals cpm.ClaveCnis
            where cpm.Cluesimb != null && cpm.Cluesimb.ToUpper() == normalizedCluesimb
            orderby article.Clave
            select new ArticuloSolicitudDto(
                article.Clave ?? string.Empty,
                article.Descripcion ?? string.Empty,
                article.Presentacion ?? string.Empty))
            .ToListAsync(cancellationToken);

        return new BuscarArticulosResponse(results, results.Count);
    }

    public async Task<CpmRowsResponse<CpmExpectedVsRowDto>> GetExpectedVsCpmAsync(
        string? cluesimb,
        string? cluessa,
        string? kit,
        string? clave,
        CancellationToken cancellationToken)
    {
        var resolvedCluesimb = await ResolveCluesimbAsync(cluesimb, cluessa, cancellationToken);
        if (string.IsNullOrWhiteSpace(resolvedCluesimb))
        {
            throw new ArgumentException("Se requiere cluesimb o cluessa.");
        }

        var filtered = dbContext.VUnidadKitClavesExpectedVsCpmV2s.AsNoTracking()
            .Where(item => item.Cluesimb != null && item.Cluesimb.ToUpper() == resolvedCluesimb);

        if (!string.IsNullOrWhiteSpace(kit))
        {
            var normalizedKit = kit.Trim().ToUpperInvariant();
            filtered = filtered.Where(item =>
                (item.KitCodigo ?? string.Empty).ToUpper() == normalizedKit ||
                (item.KitCodigos != null && item.KitCodigos.Contains(normalizedKit)));
        }

        if (!string.IsNullOrWhiteSpace(clave))
        {
            var normalizedClave = clave.Trim().ToUpperInvariant();
            filtered = filtered.Where(item => (item.ClaveCnis ?? string.Empty).ToUpper() == normalizedClave);
        }

        var rows = await filtered
            .OrderBy(item => item.KitCodigo)
            .ThenBy(item => item.ClaveCnis)
            .Select(item => new CpmExpectedVsRowDto(
                item.UnidadMedicaId,
                item.Cluesimb,
                item.Cluessa,
                item.NombreUnidad,
                item.NombreTipologia,
                item.KitCodigo,
                item.KitIds,
                item.KitCodigos,
                item.KitCodigosTxt,
                item.ClaveCnis,
                item.Cpm ?? 0m,
                item.EnCpm ?? false,
                item.Fuentes))
            .ToListAsync(cancellationToken);

        return new CpmRowsResponse<CpmExpectedVsRowDto>(rows.Count, rows);
    }

    public async Task<CpmRowsResponse<CpmUnidadRowDto>> GetUnidadCpmAsync(
        string? cluesimb,
        string? cluessa,
        CancellationToken cancellationToken)
    {
        var resolvedCluesimb = await ResolveCluesimbAsync(cluesimb, cluessa, cancellationToken);
        if (string.IsNullOrWhiteSpace(resolvedCluesimb))
        {
            throw new ArgumentException("Se requiere cluesimb o cluessa.");
        }

        var rows = await dbContext.VUnidadCpms.AsNoTracking()
            .Where(item => item.Cluesimb != null && item.Cluesimb.ToUpper() == resolvedCluesimb && (item.Cpm ?? 0m) > 0m)
            .OrderBy(item => item.ClaveCnis)
            .Select(item => new CpmUnidadRowDto(
                item.ClaveCnis ?? string.Empty,
                item.Cpm ?? 0m))
            .ToListAsync(cancellationToken);

        return new CpmRowsResponse<CpmUnidadRowDto>(rows.Count, rows);
    }

    public async Task<CpmRowsResponse<CpmEditorRowDto>> GetUnidadCpmAllAsync(
        string? cluesimb,
        string? cluessa,
        CancellationToken cancellationToken)
    {
        var resolvedCluesimb = await ResolveCluesimbAsync(cluesimb, cluessa, cancellationToken);
        if (string.IsNullOrWhiteSpace(resolvedCluesimb))
        {
            throw new ArgumentException("Se requiere cluesimb o cluessa.");
        }

        var rows = await dbContext.VUnidadCpms.AsNoTracking()
            .Where(item => item.Cluesimb != null && item.Cluesimb.ToUpper() == resolvedCluesimb)
            .OrderBy(item => item.ClaveCnis)
            .Select(item => new CpmEditorRowDto(
                item.ClaveCnis ?? string.Empty,
                item.Cpm ?? 0m,
                null))
            .ToListAsync(cancellationToken);

        return new CpmRowsResponse<CpmEditorRowDto>(rows.Count, rows);
    }

    public async Task<CpmRowsResponse<CpmEditorRowDto>> GetUnidadCpmRealAllAsync(
        string? cluesimb,
        string? cluessa,
        CancellationToken cancellationToken)
    {
        var resolvedCluesimb = await ResolveCluesimbAsync(cluesimb, cluessa, cancellationToken);
        if (string.IsNullOrWhiteSpace(resolvedCluesimb))
        {
            throw new ArgumentException("Se requiere cluesimb o cluessa.");
        }

        var rows = await dbContext.VCpmReals.AsNoTracking()
            .Where(item => item.Cluesimb != null && item.Cluesimb.ToUpper() == resolvedCluesimb)
            .OrderBy(item => item.ClaveCnis)
            .Select(item => new CpmEditorRowDto(
                item.ClaveCnis ?? string.Empty,
                item.Cpm ?? 0m,
                item.Fuente))
            .ToListAsync(cancellationToken);

        return new CpmRowsResponse<CpmEditorRowDto>(rows.Count, rows);
    }

    public async Task<ExistenciaRowsResponse> GetExistenciasByUnidadAsync(
        string cluesimb,
        CancellationToken cancellationToken)
    {
        var normalizedCluesimb = NormalizeKey(cluesimb);
        if (string.IsNullOrWhiteSpace(normalizedCluesimb))
        {
            throw new ArgumentException("cluesimb es requerido.");
        }

        var rows = await (
            from tmp in dbContext.TmpExistencias.AsNoTracking()
            join alias in dbContext.UnidadMedicaAliases.AsNoTracking()
                on (tmp.AliasSas ?? string.Empty).ToLower() equals (alias.AliasSas ?? string.Empty).ToLower() into aliasJoin
            from alias in aliasJoin.DefaultIfEmpty()
            join unidadByAlias in dbContext.UnidadMedicas.AsNoTracking()
                on alias.UnidadMedicaId equals unidadByAlias.Id into aliasUnidadJoin
            from unidadByAlias in aliasUnidadJoin.DefaultIfEmpty()
            join unidadByCluessa in dbContext.UnidadMedicas.AsNoTracking()
                on tmp.Cluessa equals unidadByCluessa.Cluessa into cluessaJoin
            from unidadByCluessa in cluessaJoin.DefaultIfEmpty()
            let resolvedCluesimb =
                !string.IsNullOrWhiteSpace(tmp.Cluesimb) ? tmp.Cluesimb!.Trim().ToUpper() :
                unidadByAlias != null && unidadByAlias.Cluesimb != null ? unidadByAlias.Cluesimb.ToUpper() :
                unidadByCluessa != null && unidadByCluessa.Cluesimb != null ? unidadByCluessa.Cluesimb.ToUpper() :
                null
            where resolvedCluesimb == normalizedCluesimb
            group tmp by tmp.ClaveCnis into grouped
            join articulo in dbContext.Articulos.AsNoTracking()
                on grouped.Key equals articulo.Clave into articuloJoin
            from articulo in articuloJoin.DefaultIfEmpty()
            select new ExistenciaUnidadRowDto(
                grouped.Key,
                articulo != null ? articulo.Descripcion ?? string.Empty : string.Empty,
                grouped.Sum(item => item.Existencia)))
            .OrderBy(item => item.ClaveCnis)
            .ToListAsync(cancellationToken);

        return new ExistenciaRowsResponse(rows);
    }

    public async Task<TemporalExistenciaRowsResponse> GetExistenciasAlmacenesFullAsync(
        CancellationToken cancellationToken)
    {
        var rows = await (
            from tmp in dbContext.TmpExistencias.AsNoTracking()
            join unidad in dbContext.VUnidadMedicaDetalles.AsNoTracking()
                on tmp.Cluesimb equals unidad.Cluesimb
            where unidad.TipoUnidad == "ALMACENES"
                && tmp.Existencia > 0m
            orderby tmp.AliasSas, tmp.ClaveCnis, tmp.Lote, tmp.FechaCaducidad
            select new TemporalExistenciaRowDto(
                tmp.Fuente,
                tmp.AliasSas,
                tmp.Cluessa,
                tmp.Cluesimb,
                tmp.ClaveCnis,
                tmp.Lote,
                tmp.FechaCaducidad,
                tmp.Existencia))
            .ToListAsync(cancellationToken);

        return new TemporalExistenciaRowsResponse(rows.Count, rows);
    }

    public async Task<HomologoRowsResponse> GetHomologosByClaveAsync(
        string clave,
        CancellationToken cancellationToken)
    {
        var normalized = NormalizeKey(clave);
        if (normalized == null)
        {
            throw new ArgumentException("clave es requerida.");
        }

        var rows = await dbContext.Homologos.AsNoTracking()
            .Where(item => item.Clave.ToUpper() == normalized)
            .OrderBy(item => item.Sustituto)
            .Select(item => new HomologoEdgeDto(
                normalized,
                item.Sustituto.ToUpper(),
                item.Factor.ToString(),
                "FORWARD"))
            .ToListAsync(cancellationToken);

        return new HomologoRowsResponse(rows);
    }

    public async Task<HomologoRowsResponse> GetHomologosBatchAsync(
        IReadOnlyList<string> claves,
        CancellationToken cancellationToken)
    {
        var normalized = NormalizeDistinctKeys(claves);
        if (normalized.Count == 0)
        {
            return new HomologoRowsResponse([]);
        }

        var edges = await dbContext.Homologos.AsNoTracking()
            .Where(item => normalized.Contains(item.Clave.ToUpper()) || normalized.Contains(item.Sustituto.ToUpper()))
            .ToListAsync(cancellationToken);

        var rows = edges
            .SelectMany(edge =>
            {
                var clave = edge.Clave.ToUpper();
                var sustituto = edge.Sustituto.ToUpper();
                var list = new List<HomologoEdgeDto>(2);
                if (normalized.Contains(clave))
                {
                    list.Add(new HomologoEdgeDto(clave, sustituto, edge.Factor.ToString(), "FORWARD"));
                }
                if (normalized.Contains(sustituto) && edge.Factor != 0)
                {
                    list.Add(new HomologoEdgeDto(sustituto, clave, (1m / edge.Factor).ToString(), "REVERSE"));
                }
                return list;
            })
            .OrderBy(item => item.ClaveConsultada)
            .ThenBy(item => item.Direccion)
            .ThenBy(item => item.Candidato)
            .ToList();

        return new HomologoRowsResponse(rows);
    }

    public async Task<HomologoRowsResponse> GetHomologosBatchForwardAsync(
        IReadOnlyList<string> claves,
        CancellationToken cancellationToken)
    {
        var normalized = NormalizeDistinctKeys(claves);
        if (normalized.Count == 0)
        {
            return new HomologoRowsResponse([]);
        }

        var rows = await dbContext.Homologos.AsNoTracking()
            .Where(item => normalized.Contains(item.Clave.ToUpper()))
            .OrderBy(item => item.Clave)
            .ThenBy(item => item.Sustituto)
            .Select(item => new HomologoEdgeDto(
                item.Clave.ToUpper(),
                item.Sustituto.ToUpper(),
                item.Factor.ToString(),
                "FORWARD"))
            .ToListAsync(cancellationToken);

        return new HomologoRowsResponse(rows);
    }

    public async Task<EffectiveFlagsResponse> GetEffectiveFlagsAsync(
        string? cluesimb,
        string? nivel,
        CancellationToken cancellationToken)
    {
        var normalizedCluesimb = NormalizeKey(cluesimb);
        var normalizedNivel = NormalizeNivel(nivel);

        var rows = await dbContext.FeatureFlags.AsNoTracking()
            .Where(item =>
                KnownFlags.Contains(item.FlagKey) &&
                (
                    item.Scope == "global" ||
                    (normalizedNivel != null && item.Scope == "nivel" && item.ScopeId == normalizedNivel) ||
                    (normalizedCluesimb != null && item.Scope == "clues" && item.ScopeId != null && item.ScopeId.ToUpper() == normalizedCluesimb)
                ))
            .ToListAsync(cancellationToken);

        var flags = rows
            .OrderBy(item => GetScopePrecedence(item.Scope))
            .ThenBy(item => item.FlagKey, StringComparer.Ordinal)
            .Aggregate(new Dictionary<string, object?>(StringComparer.Ordinal), (acc, item) =>
            {
                acc[item.FlagKey] = ParseFlagValue(item.ValueJson);
                return acc;
            });

        return new EffectiveFlagsResponse(true, flags);
    }

    public async Task<ListFeatureFlagsResponse> ListFeatureFlagsAsync(CancellationToken cancellationToken)
    {
        var rows = await dbContext.FeatureFlags.AsNoTracking()
            .OrderBy(item => item.FlagKey)
            .ThenBy(item => item.Scope)
            .ThenBy(item => item.ScopeId ?? string.Empty)
            .Select(item => new FeatureFlagRowDto(
                item.Id,
                item.FlagKey,
                item.Scope,
                item.ScopeId,
                ParseJsonNode(item.ValueJson),
                item.Description,
                item.UpdatedBy,
                item.UpdatedAt))
            .ToListAsync(cancellationToken);

        return new ListFeatureFlagsResponse(true, rows);
    }

    public async Task<UpsertFeatureFlagsResponse> UpsertFeatureFlagsAsync(
        IReadOnlyList<UpsertFeatureFlagRequest> requests,
        string? updatedBy,
        CancellationToken cancellationToken)
    {
        if (requests.Count == 0)
        {
            throw new ArgumentException("Se requiere al menos un cambio.");
        }

        var rows = new List<FeatureFlagRowDto>(requests.Count);
        foreach (var request in requests)
        {
            var normalized = NormalizeUpsertRequest(request);
            var entity = await dbContext.FeatureFlags
                .SingleOrDefaultAsync(item =>
                    item.FlagKey == normalized.FlagKey &&
                    item.Scope == normalized.Scope &&
                    item.ScopeId == normalized.ScopeId, cancellationToken);

            if (entity is null)
            {
                entity = new FeatureFlag
                {
                    FlagKey = normalized.FlagKey,
                    Scope = normalized.Scope,
                    ScopeId = normalized.ScopeId,
                    ValueJson = normalized.ValueJson,
                    UpdatedBy = NormalizeUpdatedBy(updatedBy)
                };
                dbContext.FeatureFlags.Add(entity);
            }
            else
            {
                entity.ValueJson = normalized.ValueJson;
                entity.UpdatedBy = NormalizeUpdatedBy(updatedBy);
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var request in requests)
        {
            var normalized = NormalizeUpsertRequest(request);
            var entity = await dbContext.FeatureFlags.AsNoTracking()
                .SingleAsync(item =>
                    item.FlagKey == normalized.FlagKey &&
                    item.Scope == normalized.Scope &&
                    item.ScopeId == normalized.ScopeId, cancellationToken);

            rows.Add(new FeatureFlagRowDto(
                entity.Id,
                entity.FlagKey,
                entity.Scope,
                entity.ScopeId,
                ParseJsonNode(entity.ValueJson),
                entity.Description,
                entity.UpdatedBy,
                entity.UpdatedAt));
        }

        return new UpsertFeatureFlagsResponse(true, rows.Count, rows);
    }

    public async Task<IReadOnlyList<UnidadAllowlistDto>> GetAllowlistUnidadesAsync(
        string? query,
        CancellationToken cancellationToken)
    {
        var effective = await GetEffectiveFlagsAsync(null, null, cancellationToken);
        if (!effective.Flags.TryGetValue("CLUES_EXISTENCIAS_ALLOWLIST", out var rawAllowlist))
        {
            return [];
        }

        var aliases = ExtractAllowlistAliases(rawAllowlist);
        if (aliases.Count == 0)
        {
            return [];
        }

        var normalizedQuery = string.IsNullOrWhiteSpace(query) ? null : query.Trim();
        var rows = await dbContext.UnidadMedicaAliases.AsNoTracking()
            .Where(item => item.AliasDash != null && aliases.Contains(item.AliasDash))
            .Select(item => new
            {
                item.AliasDash,
                item.UnidadMedica.Cluesimb,
                item.UnidadMedica.Nombre
            })
            .Where(item =>
                normalizedQuery == null ||
                EF.Functions.ILike(item.Cluesimb ?? string.Empty, $"%{normalizedQuery}%") ||
                EF.Functions.ILike(item.AliasDash ?? string.Empty, $"%{normalizedQuery}%") ||
                EF.Functions.ILike(item.Nombre ?? string.Empty, $"%{normalizedQuery}%"))
            .OrderBy(item => item.AliasDash)
            .ThenBy(item => item.Nombre)
            .Take(100)
            .Select(item => new UnidadAllowlistDto(
                item.Cluesimb ?? string.Empty,
                item.Nombre ?? string.Empty,
                item.AliasDash ?? string.Empty))
            .ToListAsync(cancellationToken);

        return rows;
    }

    public async Task<CrearBitacoraResponse> CrearBitacoraAsync(
        CrearBitacoraRequest request,
        CancellationToken cancellationToken)
    {
        var canonical = Canonicalize(request);
        var hash = ComputeHash(canonical);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var existingId = await dbContext.SolicitudBitacoras.AsNoTracking()
            .Where(item => item.Cluesimb == canonical.Cluesimb && item.CreatedDay == today && item.PayloadHash == hash)
            .Select(item => (Guid?)item.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (existingId.HasValue)
        {
            return new CrearBitacoraResponse(true, existingId.Value, true, hash);
        }

        var header = new SolicitudBitacora
        {
            Cluesimb = canonical.Cluesimb,
            TipoPedido = canonical.TipoPedido,
            TiposInsumo = canonical.TiposInsumo,
            PeriodoTexto = string.IsNullOrWhiteSpace(canonical.Periodo) ? null : canonical.Periodo,
            ExportKind = "raw",
            TotalRenglones = canonical.Articulos.Count,
            TotalPiezas = canonical.Articulos.Sum(item => item.Cantidad),
            PayloadHash = hash
        };
        header.SolicitudBitacoraDetalles = canonical.Articulos.Select(item => new SolicitudBitacoraDetalle
        {
            Clave = item.Clave,
            Cantidad = item.Cantidad,
            UnidadMedida = null
        }).ToList();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        dbContext.SolicitudBitacoras.Add(header);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new CrearBitacoraResponse(true, header.Id, false, hash);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            await transaction.RollbackAsync(cancellationToken);
            dbContext.ChangeTracker.Clear();
            var concurrentId = await dbContext.SolicitudBitacoras.AsNoTracking()
                .Where(item => item.Cluesimb == canonical.Cluesimb && item.CreatedDay == today && item.PayloadHash == hash)
                .Select(item => item.Id)
                .SingleAsync(cancellationToken);
            return new CrearBitacoraResponse(true, concurrentId, true, hash);
        }
    }

    private CanonicalRequest Canonicalize(CrearBitacoraRequest request)
    {
        var articles = request.Articulos
            .Select(item => new CanonicalArticle(item.Clave.Trim().ToUpperInvariant(), item.Cantidad))
            .OrderBy(item => item.Clave, StringComparer.Ordinal)
            .ToList();
        var types = request.TipoInsumo.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(CollapseWhitespace).Distinct(StringComparer.Ordinal).OrderBy(item => item, StringComparer.Ordinal).ToList();
        return new CanonicalRequest(
            request.Cluesimb.Trim().ToUpperInvariant(),
            CollapseWhitespace(request.TipoPedido),
            types,
            CollapseWhitespace(request.Periodo ?? string.Empty),
            articles);
    }

    private IQueryable<VUnidadMedicaDetalle> BuildUnidadesQuery(string? query, string? nivel)
    {
        var normalizedNivel = NormalizeNivel(nivel);
        var filtered = dbContext.VUnidadMedicaDetalles.AsNoTracking().AsQueryable();

        if (normalizedNivel is "PRIMER_NIVEL")
        {
            filtered = filtered.Where(item => item.EsSegundoNivel == false || item.NivelAtencion == "PRIMER NIVEL");
        }
        else if (normalizedNivel is "SEGUNDO_NIVEL")
        {
            filtered = filtered.Where(item => item.EsSegundoNivel == true || item.NivelAtencion == "SEGUNDO NIVEL");
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim();
            filtered = filtered.Where(item =>
                EF.Functions.ILike(item.Cluesimb ?? string.Empty, $"%{normalizedQuery}%") ||
                EF.Functions.ILike(item.Cluessa ?? string.Empty, $"%{normalizedQuery}%") ||
                EF.Functions.ILike(item.NombreDeUnidad ?? string.Empty, $"%{normalizedQuery}%") ||
                EF.Functions.ILike(item.NombreMunicipio ?? string.Empty, $"%{normalizedQuery}%") ||
                EF.Functions.ILike(item.NombreLocalidad ?? string.Empty, $"%{normalizedQuery}%") ||
                EF.Functions.ILike(item.AliasSas ?? string.Empty, $"%{normalizedQuery}%"));
        }

        return filtered;
    }

    private static UnidadExistenteDto MapUnidadExistente(VUnidadMedicaDetalle item, string tipoUnidad) =>
        new(
            item.Cluesimb ?? string.Empty,
            item.Cluessa,
            item.Cluesimb,
            item.NombreDeUnidad ?? string.Empty,
            item.NombreMunicipio ?? string.Empty,
            item.NombreLocalidad ?? string.Empty,
            MapJurisdiccion(item.NombreMunicipio),
            item.Direccion ?? string.Empty,
            item.Latitud?.ToString() ?? string.Empty,
            item.Longitud?.ToString() ?? string.Empty,
            item.EstratoUnidad ?? string.Empty,
            item.NivelAtencion ?? string.Empty,
            tipoUnidad);

    private static string MapJurisdiccion(string? municipio) => (municipio ?? string.Empty).ToUpperInvariant() switch
    {
        "TIJUANA" or "TECATE" or "PLAYAS DE ROSARITO" => "TIJUANA",
        "MEXICALI" or "SAN FELIPE" => "MEXICALI",
        "ENSENADA" or "SAN QUINTIN" => "ENSENADA",
        _ => municipio ?? string.Empty
    };

    private async Task<string?> ResolveCluesimbAsync(string? cluesimb, string? cluessa, CancellationToken cancellationToken)
    {
        var normalizedCluesimb = NormalizeKey(cluesimb);
        if (normalizedCluesimb != null)
        {
            return normalizedCluesimb;
        }

        var normalizedCluessa = NormalizeKey(cluessa);
        if (normalizedCluessa == null)
        {
            return null;
        }

        return await dbContext.VUnidadMedicaDetalles.AsNoTracking()
            .Where(item => item.Cluessa != null && item.Cluessa.ToUpper() == normalizedCluessa)
            .Select(item => item.Cluesimb)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static string? NormalizeNivel(string? nivel) => (nivel ?? string.Empty).Trim().ToUpperInvariant() switch
    {
        "PRIMER_NIVEL" => "PRIMER_NIVEL",
        "SEGUNDO_NIVEL" => "SEGUNDO_NIVEL",
        _ => null
    };

    private static string? NormalizeKey(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();

    private static HashSet<string> NormalizeDistinctKeys(IEnumerable<string> values) =>
        values.Select(NormalizeKey).Where(value => value != null).Cast<string>().ToHashSet(StringComparer.Ordinal);

    private static int GetScopePrecedence(string scope) => scope switch
    {
        "global" => 1,
        "nivel" => 2,
        "clues" => 3,
        _ => 4
    };

    private static object? ParseFlagValue(string? rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return null;
        }

        using var document = JsonDocument.Parse(rawJson);
        if (document.RootElement.ValueKind == JsonValueKind.Object &&
            document.RootElement.TryGetProperty("bool", out var boolProperty) &&
            (boolProperty.ValueKind == JsonValueKind.True || boolProperty.ValueKind == JsonValueKind.False))
        {
            return boolProperty.GetBoolean();
        }

        return JsonNode.Parse(rawJson);
    }

    private static JsonNode? ParseJsonNode(string? rawJson) =>
        string.IsNullOrWhiteSpace(rawJson) ? null : JsonNode.Parse(rawJson);

    private static NormalizedUpsertFlag NormalizeUpsertRequest(UpsertFeatureFlagRequest request)
    {
        var flagKey = (request.FlagKey ?? string.Empty).Trim();
        if (!KnownFlags.Contains(flagKey, StringComparer.Ordinal))
        {
            throw new ArgumentException($"flag_key inválido: {request.FlagKey}");
        }

        var scope = (request.Scope ?? string.Empty).Trim().ToLowerInvariant();
        if (scope is not ("global" or "nivel" or "clues"))
        {
            throw new ArgumentException("scope inválido");
        }

        if (request.Value is null)
        {
            throw new ArgumentException("value es requerido");
        }

        if (BooleanFlags.Contains(flagKey) && request.Value.GetValueKind() is not (JsonValueKind.True or JsonValueKind.False))
        {
            throw new ArgumentException($"{flagKey} debe ser boolean");
        }

        var scopeId = scope == "global"
            ? "global"
            : NormalizeScopedId(request.ScopeId);

        if (scope != "global" && string.IsNullOrWhiteSpace(scopeId))
        {
            throw new ArgumentException("scope_id es requerido para nivel o clues");
        }

        var serializedValue = BooleanFlags.Contains(flagKey)
            ? JsonSerializer.Serialize(new { @bool = request.Value.GetValue<bool>() })
            : request.Value.ToJsonString();

        return new NormalizedUpsertFlag(flagKey, scope, scopeId, serializedValue);
    }

    private static string? NormalizeScopedId(string? scopeId) =>
        string.IsNullOrWhiteSpace(scopeId) ? null : scopeId.Trim().ToUpperInvariant();

    private static string NormalizeUpdatedBy(string? updatedBy) =>
        string.IsNullOrWhiteSpace(updatedBy) ? "api" : updatedBy.Trim();

    private static HashSet<string> ExtractAllowlistAliases(object? rawAllowlist)
    {
        var aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        switch (rawAllowlist)
        {
            case JsonArray jsonArray:
                foreach (var item in jsonArray.OfType<JsonValue>())
                {
                    var alias = item.TryGetValue<string>(out var value) ? value : null;
                    if (!string.IsNullOrWhiteSpace(alias))
                    {
                        aliases.Add(alias.Trim());
                    }
                }
                break;
            case JsonObject jsonObject when jsonObject["list"] is JsonArray list:
                foreach (var item in list.OfType<JsonValue>())
                {
                    var alias = item.TryGetValue<string>(out var value) ? value : null;
                    if (!string.IsNullOrWhiteSpace(alias))
                    {
                        aliases.Add(alias.Trim());
                    }
                }
                break;
        }

        return aliases;
    }

    private sealed record NormalizedUpsertFlag(string FlagKey, string Scope, string? ScopeId, string ValueJson);

    private string ComputeHash(CanonicalRequest request)
    {
        var salt = Environment.GetEnvironmentVariable("SOLICITUDES_HASH_SALT")
            ?? configuration["Solicitudes:HashSalt"];
        if (string.IsNullOrWhiteSpace(salt))
        {
            throw new InvalidOperationException("SOLICITUDES_HASH_SALT no está configurado.");
        }
        var json = JsonSerializer.Serialize(new
        {
            cluesimb = request.Cluesimb,
            tipoPedido = request.TipoPedido,
            tiposInsumo = request.TiposInsumo,
            periodo = request.Periodo,
            articulos = request.Articulos.Select(item => new { clave = item.Clave, cantidad = item.Cantidad })
        });
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{json}|{salt}"))).ToLowerInvariant();
    }

    private static string CollapseWhitespace(string value) => string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    private sealed record CanonicalArticle(string Clave, decimal Cantidad);
    private sealed record CanonicalRequest(string Cluesimb, string TipoPedido, List<string> TiposInsumo, string Periodo, List<CanonicalArticle> Articulos);
}

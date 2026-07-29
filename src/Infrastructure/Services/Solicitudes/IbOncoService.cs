using System.Globalization;
using System.Net.Http.Headers;
using Application.Features.Solicitudes;
using Domain.Entities.Solicitudes;
using Infrastructure.Persistence.Solicitudes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Solicitudes;

internal sealed class IbOncoService(
    SolicitudesDbContext dbContext,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : IIbOncoService
{
    private const string SaciaFuente = "sacia-onco";
    private static readonly SaciaUnidad[] SaciaUnidades =
    [
        new(1, "BCIMB000010", "Hospital General de Ensenada"),
        new(2, "BCIMB000355", "Hospital General de Mexicali"),
        new(3, "BCIMB000734", "Hospital General de Tijuana"),
        new(4, "BCIMB001726", "Uneme Oncologia Mexicali")
    ];

    public async Task<IbOncoListResponse<IbOncoUnidadDto>> GetUnidadesAsync(CancellationToken cancellationToken)
    {
        var rows = await (
            from unidad in dbContext.OncoUnidades.AsNoTracking()
            join detalle in dbContext.VUnidadMedicaDetalles.AsNoTracking()
                on unidad.Cluesimb equals detalle.Cluesimb into detalleJoin
            from detalle in detalleJoin.DefaultIfEmpty()
            orderby detalle!.NombreDeUnidad, unidad.Cluesimb
            select new IbOncoUnidadDto(
                unidad.Id,
                unidad.Cluesimb,
                detalle!.Cluessa,
                detalle.NombreDeUnidad,
                detalle.NombreMunicipio))
            .ToListAsync(cancellationToken);

        return new IbOncoListResponse<IbOncoUnidadDto>(true, rows.Count, rows);
    }

    public async Task<IbOncoListResponse<IbOncoClaveDto>> GetClavesAsync(string? cluesimb, CancellationToken cancellationToken)
    {
        var normalizedCluesimb = NormalizeKey(cluesimb);

        var rows = await (
            from clave in dbContext.OncoClavesBases.AsNoTracking()
            join articulo in dbContext.Articulos.AsNoTracking()
                on clave.ClaveCnis equals articulo.Clave into articuloJoin
            from articulo in articuloJoin.DefaultIfEmpty()
            where normalizedCluesimb == null || clave.Cluesimb == normalizedCluesimb
            orderby clave.Cluesimb, clave.ClaveCnis
            select new IbOncoClaveDto(
                clave.Id,
                clave.Cluesimb,
                clave.ClaveCnis,
                articulo!.Descripcion))
            .ToListAsync(cancellationToken);

        return new IbOncoListResponse<IbOncoClaveDto>(true, rows.Count, rows);
    }

    public async Task<IbOncoPaginatedResponse<IbOncoAbastoCpmRowDto>> GetAbastoCpmAsync(
        string? cluesimb,
        string? claveCnis,
        string? estadoAbasto,
        string? search,
        int? windowDays,
        int? page,
        int? limit,
        int? offset,
        CancellationToken cancellationToken)
    {
        var normalizedCluesimb = NormalizeKey(cluesimb);
        var normalizedClave = NormalizeKey(claveCnis);
        var normalizedEstado = string.IsNullOrWhiteSpace(estadoAbasto) ? null : estadoAbasto.Trim();
        var normalizedSearch = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pagination = IbOncoPagination.Normalize(page, limit, offset);

        var cutoff = DateOnly.FromDateTime(DateTime.Today.AddDays(-NormalizeWindowDays(windowDays)));

        var citasLookup = await dbContext.Citas.AsNoTracking()
            .Where(item => item.FechaRecepcionMax == null
                && item.FechaEmision != null
                && item.FechaEmision >= cutoff)
            .Join(
                dbContext.OncoClavesBases.AsNoTracking(),
                cita => new { Cluesimb = cita.CluesDestino!, ClaveCnis = cita.ClaveCnis! },
                onco => new { Cluesimb = onco.Cluesimb, ClaveCnis = onco.ClaveCnis },
                (cita, _) => cita)
            .GroupBy(item => new { item.CluesDestino, item.ClaveCnis })
            .Select(group => new
            {
                group.Key.CluesDestino,
                group.Key.ClaveCnis,
                CitasPendientes = group.Count(),
                PiezasPendientes = group.Sum(item => item.NoDePiezasEmitidas ?? 0)
            })
            .ToDictionaryAsync(
                item => $"{item.CluesDestino}|{item.ClaveCnis}",
                item => new { item.CitasPendientes, PiezasPendientes = Convert.ToDecimal(item.PiezasPendientes) },
                cancellationToken);

        var query =
            from view in dbContext.VOncoAbastoCpms.AsNoTracking()
            join articulo in dbContext.Articulos.AsNoTracking()
                on view.ClaveCnis equals articulo.Clave into articuloJoin
            from articulo in articuloJoin.DefaultIfEmpty()
            where (normalizedCluesimb == null || view.Cluesimb == normalizedCluesimb)
                && (normalizedClave == null || view.ClaveCnis == normalizedClave)
                && (normalizedEstado == null || view.EstadoAbasto == normalizedEstado)
                && (normalizedSearch == null
                    || (view.Cluesimb != null && EF.Functions.ILike(view.Cluesimb, $"%{normalizedSearch}%"))
                    || (view.NombreDeUnidad != null && EF.Functions.ILike(view.NombreDeUnidad, $"%{normalizedSearch}%"))
                    || (view.ClaveCnis != null && EF.Functions.ILike(view.ClaveCnis, $"%{normalizedSearch}%"))
                    || (articulo!.Descripcion != null && EF.Functions.ILike(articulo.Descripcion, $"%{normalizedSearch}%")))
            select new
            {
                view.Cluesimb,
                view.NombreDeUnidad,
                view.ClaveCnis,
                Descripcion = articulo!.Descripcion,
                view.Existencias,
                view.Cpm,
                view.CpmX3,
                view.CpmsEq,
                view.EstadoAbasto
            };

        var total = await query.CountAsync(cancellationToken);

        var pageRows = await query
            .OrderBy(item => item.EstadoAbasto == "posible sobre abasto" ? 0 : 1)
            .ThenBy(item => item.Cluesimb)
            .ThenBy(item => item.ClaveCnis)
            .Skip(pagination.Offset)
            .Take(pagination.Limit)
            .ToListAsync(cancellationToken);

        var rows = pageRows.Select(item =>
        {
            var key = $"{item.Cluesimb}|{item.ClaveCnis}";
            var cita = citasLookup.TryGetValue(key, out var value)
                ? value
                : null;

            var citasPendientes = cita?.CitasPendientes ?? 0;
            var piezasPendientes = cita?.PiezasPendientes ?? 0m;

            return new IbOncoAbastoCpmRowDto(
                item.Cluesimb ?? string.Empty,
                item.NombreDeUnidad,
                item.ClaveCnis ?? string.Empty,
                item.Descripcion,
                item.Existencias ?? 0m,
                item.Cpm ?? 0m,
                item.CpmX3 ?? 0m,
                item.CpmsEq ?? 0m,
                item.EstadoAbasto,
                citasPendientes,
                piezasPendientes,
                citasPendientes > 0);
        }).ToList();

        return Paginate(rows, total, pagination.Page, pagination.Limit, pagination.Offset);
    }

    public async Task<IbOncoPaginatedResponse<IbOncoCitaPendienteRowDto>> GetCitasPendientesAsync(
        string? cluesimb,
        string? claveCnis,
        int? windowDays,
        int? page,
        int? limit,
        int? offset,
        CancellationToken cancellationToken)
    {
        var normalizedCluesimb = NormalizeKey(cluesimb);
        var normalizedClave = NormalizeKey(claveCnis);
        var pagination = IbOncoPagination.Normalize(page, limit, offset);
        var cutoff = DateOnly.FromDateTime(DateTime.Today.AddDays(-NormalizeWindowDays(windowDays)));

        var query =
            from cita in dbContext.Citas.AsNoTracking()
            join onco in dbContext.OncoClavesBases.AsNoTracking()
                on new { Cluesimb = cita.CluesDestino!, ClaveCnis = cita.ClaveCnis! }
                equals new { Cluesimb = onco.Cluesimb, ClaveCnis = onco.ClaveCnis }
            where cita.FechaRecepcionMax == null
                && cita.FechaEmision != null
                && cita.FechaEmision >= cutoff
                && (normalizedCluesimb == null || cita.CluesDestino == normalizedCluesimb)
                && (normalizedClave == null || cita.ClaveCnis == normalizedClave)
            select cita;

        var total = await query.CountAsync(cancellationToken);

        var rows = await query
            .OrderByDescending(item => item.FechaLimiteDeEntrega)
            .ThenByDescending(item => item.Id)
            .Skip(pagination.Offset)
            .Take(pagination.Limit)
            .Select(item => new IbOncoCitaPendienteRowDto(
                item.Id,
                item.Ejercicio,
                item.OrdenDeSuministro,
                item.Institucion,
                item.Contrato,
                item.CluesDestino ?? string.Empty,
                item.Unidad,
                item.ClaveCnis ?? string.Empty,
                item.Descripcion,
                item.Proveedor,
                item.Compra,
                item.TipoDeEntrega,
                item.FteFmto,
                item.TipoDeRed,
                item.TipoDeInsumo,
                item.GrupoTerapeutico,
                item.PrecioUnitario ?? 0m,
                item.NoDePiezasEmitidas ?? 0,
                item.PzasRecibidasPorLaEntidad ?? 0m,
                item.FechaEmision,
                item.FechaLimiteDeEntrega,
                item.FechaDeCita,
                item.Estatus,
                item.FolioAbasto))
            .ToListAsync(cancellationToken);

        return Paginate(rows, total, pagination.Page, pagination.Limit, pagination.Offset);
    }

    public async Task<IbOncoListResponse<IbOncoResumenUnidadDto>> GetResumenAsync(int? windowDays, CancellationToken cancellationToken)
    {
        var cutoff = DateOnly.FromDateTime(DateTime.Today.AddDays(-NormalizeWindowDays(windowDays)));

        var citasLookup = await dbContext.Citas.AsNoTracking()
            .Where(item => item.FechaRecepcionMax == null
                && item.FechaEmision != null
                && item.FechaEmision >= cutoff)
            .Join(
                dbContext.OncoClavesBases.AsNoTracking(),
                cita => new { Cluesimb = cita.CluesDestino!, ClaveCnis = cita.ClaveCnis! },
                onco => new { Cluesimb = onco.Cluesimb, ClaveCnis = onco.ClaveCnis },
                (cita, _) => cita)
            .GroupBy(item => new { item.CluesDestino, item.ClaveCnis })
            .Select(group => new
            {
                group.Key.CluesDestino,
                group.Key.ClaveCnis,
                CitasPendientes = group.Count(),
                PiezasPendientes = group.Sum(item => item.NoDePiezasEmitidas ?? 0)
            })
            .ToListAsync(cancellationToken);

        var baseRows = await dbContext.VOncoAbastoCpms.AsNoTracking()
            .Select(item => new
            {
                item.Cluesimb,
                item.NombreDeUnidad,
                item.ClaveCnis,
                item.Existencias,
                item.Cpm,
                item.EstadoAbasto
            })
            .ToListAsync(cancellationToken);

        var resumen = baseRows
            .GroupBy(item => item.Cluesimb ?? string.Empty)
            .Select(group =>
            {
                var citasGrupo = citasLookup
                    .Where(item => string.Equals(item.CluesDestino, group.Key, StringComparison.Ordinal))
                    .ToList();

                return new IbOncoResumenUnidadDto(
                    group.Key,
                    group.Select(item => item.NombreDeUnidad).FirstOrDefault(item => !string.IsNullOrWhiteSpace(item)),
                    group.Count(),
                    group.Count(item => item.EstadoAbasto == "posible sobre abasto"),
                    group.Sum(item => item.Existencias ?? 0m),
                    group.Sum(item => item.Cpm ?? 0m),
                    citasGrupo.Sum(item => item.CitasPendientes),
                    citasGrupo.Sum(item => Convert.ToDecimal(item.PiezasPendientes)));
            })
            .OrderByDescending(item => item.ClavesPosibleSobreAbasto)
            .ThenByDescending(item => item.CitasPendientes)
            .ThenBy(item => item.NombreDeUnidad)
            .ToList();

        return new IbOncoListResponse<IbOncoResumenUnidadDto>(true, resumen.Count, resumen);
    }

    public async Task<IbOncoCitasXClaveResponse> GetCitasXClaveAsync(
        string? clave,
        int? windowDays,
        bool incluyeNoRecibidas,
        DateOnly? desde,
        DateOnly? hasta,
        int? limit,
        CancellationToken cancellationToken)
    {
        var normalizedClave = NormalizeKey(clave);
        if (normalizedClave == null)
        {
            throw new ArgumentException("clave es requerida.");
        }

        var days = Math.Clamp(windowDays ?? 30, 1, 365);
        var normalizedLimit = Math.Clamp(limit ?? 200, 1, 2000);
        var cutoff = DateOnly.FromDateTime(DateTime.Today.AddDays(-days));

        var query = dbContext.Citas.AsNoTracking()
            .Where(item => item.ClaveCnis == normalizedClave)
            .Where(item =>
                (item.FechaLimiteDeEntrega != null && item.FechaLimiteDeEntrega >= cutoff) ||
                (item.FechaRecepcionMax != null && item.FechaRecepcionMax >= cutoff))
            .Where(item =>
                (item.FechaRecepcionMax != null && item.FechaRecepcionMax >= cutoff) ||
                (incluyeNoRecibidas && item.FechaRecepcionMax == null));

        if (desde.HasValue)
        {
            query = query.Where(item => item.FechaRecepcionMax != null && item.FechaRecepcionMax >= desde.Value);
        }

        if (hasta.HasValue)
        {
            query = query.Where(item => item.FechaRecepcionMin != null && item.FechaRecepcionMin <= hasta.Value);
        }

        var rows = await query
            .OrderByDescending(item => item.FechaRecepcionMax ?? item.FechaLimiteDeEntrega)
            .ThenByDescending(item => item.Id)
            .Take(normalizedLimit)
            .Select(item => new IbOncoCitaXClaveDto(
                item.Id,
                item.Ejercicio,
                item.OrdenDeSuministro,
                item.Procedimiento,
                item.TipoDeEntrega,
                item.Unidad,
                item.FteFmto,
                item.Compra,
                item.NoDePiezasEmitidas ?? 0,
                item.PzasRecibidasPorLaEntidad ?? 0m,
                item.FechaEmision,
                item.FechaRecepcionLista,
                item.FechaLimiteDeEntrega,
                item.FechaDeCita,
                item.Estatus,
                item.Contrato,
                item.GrupoTerapeutico,
                item.TipoDeRed,
                item.TipoDeInsumo,
                item.Proveedor))
            .ToListAsync(cancellationToken);

        return new IbOncoCitasXClaveResponse(true, rows, rows.FirstOrDefault());
    }

    public async Task<IbOncoSaciaUpdateResponse> UpdateSaciaAsync(CancellationToken cancellationToken)
    {
        var startedAt = DateTime.UtcNow;
        var results = new List<IbOncoSaciaUnidadResult>(SaciaUnidades.Length);

        foreach (var unidad in SaciaUnidades)
        {
            try
            {
                results.Add(await UpdateSaciaUnitAsync(unidad, cancellationToken));
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                results.Add(new IbOncoSaciaUnidadResult(
                    unidad.Id,
                    unidad.Cluesimb,
                    unidad.Nombre,
                    false,
                    0,
                    0,
                    0,
                    0,
                    0,
                    exception.Message));
            }
        }

        var failed = results.Count(item => !item.Ok);
        return new IbOncoSaciaUpdateResponse(
            failed == 0,
            SaciaFuente,
            startedAt,
            DateTime.UtcNow,
            results.Count,
            results.Count - failed,
            failed,
            results.Sum(item => item.OncoClavesInsertados),
            results.Sum(item => item.TmpExistenciasEliminados),
            results.Sum(item => item.TmpExistenciasInsertados),
            results);
    }

    private static string? NormalizeKey(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToUpperInvariant();
    }

    private static int NormalizeWindowDays(int? value)
    {
        var days = value ?? 120;
        return Math.Clamp(days, 1, 365);
    }

    private async Task<IbOncoSaciaUnidadResult> UpdateSaciaUnitAsync(
        SaciaUnidad unidad,
        CancellationToken cancellationToken)
    {
        var csv = await FetchSaciaCsvAsync(unidad.Id, cancellationToken);
        var rows = ParseSaciaCsv(csv);
        var unitData = await dbContext.VUnidadMedicaDetalles.AsNoTracking()
            .Where(item => item.Cluesimb == unidad.Cluesimb)
            .Select(item => new { item.AliasSas, item.Cluessa })
            .FirstOrDefaultAsync(cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await dbContext.OncoClaves
                .Where(item => item.Cluesimb == unidad.Cluesimb)
                .ExecuteDeleteAsync(cancellationToken);
            var deletedExistencias = await dbContext.TmpExistencias
                .Where(item => item.Cluesimb == unidad.Cluesimb && item.Fuente == SaciaFuente)
                .ExecuteDeleteAsync(cancellationToken);

            dbContext.OncoClaves.AddRange(rows.Select(item => new OncoClafe
            {
                Cluesimb = unidad.Cluesimb,
                ClaveCnis = item.Clave
            }));
            dbContext.TmpExistencias.AddRange(rows.Select(item => new TmpExistencia
            {
                Fuente = SaciaFuente,
                AliasSas = unitData?.AliasSas,
                Cluessa = unitData?.Cluessa,
                Cluesimb = unidad.Cluesimb,
                ClaveCnis = item.Clave,
                Lote = string.Empty,
                FechaCaducidad = null,
                Existencia = item.Existencias
            }));

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new IbOncoSaciaUnidadResult(
                unidad.Id,
                unidad.Cluesimb,
                unidad.Nombre,
                true,
                rows.Count,
                rows.Count,
                rows.Count,
                deletedExistencias,
                rows.Count);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            dbContext.ChangeTracker.Clear();
            throw;
        }
    }

    private async Task<string> FetchSaciaCsvAsync(int unitId, CancellationToken cancellationToken)
    {
        var baseUrl = Environment.GetEnvironmentVariable("SACIA_ONCO_EXISTENCIAS_URL")
            ?? configuration["SaciaOnco:ExistenciasUrl"];
        var token = Environment.GetEnvironmentVariable("SACIA_ONCO_TOKEN")
            ?? configuration["SaciaOnco:Token"];

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("SACIA_ONCO_EXISTENCIAS_URL no está configurado.");
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("SACIA_ONCO_TOKEN no está configurado.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}{Uri.EscapeDataString(unitId.ToString(CultureInfo.InvariantCulture))}");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/csv"));
        request.Headers.TryAddWithoutValidation("Authorization", token);

        var client = httpClientFactory.CreateClient(nameof(IbOncoService));
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"SACIA ONCO respondió {(int)response.StatusCode}: {content[..Math.Min(content.Length, 250)]}");
        }

        return content;
    }

    private static List<SaciaCsvRow> ParseSaciaCsv(string csv)
    {
        var lines = csv.TrimStart('\uFEFF')
            .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var rows = new Dictionary<string, SaciaCsvRow>(StringComparer.Ordinal);

        foreach (var line in lines.Skip(1))
        {
            var columns = ParseCsvLine(line);
            var clave = NormalizeKey(columns.ElementAtOrDefault(0));
            if (clave == null)
            {
                continue;
            }

            rows[clave] = new SaciaCsvRow(
                clave,
                ParseDecimal(columns.ElementAtOrDefault(2)),
                ParseDecimal(columns.ElementAtOrDefault(3)));
        }

        return rows.Values.ToList();
    }

    private static List<string> ParseCsvLine(string line)
    {
        var values = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        for (var index = 0; index < line.Length; index++)
        {
            var character = line[index];
            if (character == '"' && inQuotes && index + 1 < line.Length && line[index + 1] == '"')
            {
                current.Append('"');
                index++;
            }
            else if (character == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (character == ',' && !inQuotes)
            {
                values.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(character);
            }
        }

        values.Add(current.ToString().Trim());
        return values;
    }

    private static decimal ParseDecimal(string? value)
    {
        var normalized = (value ?? string.Empty).Trim().Replace(",", string.Empty, StringComparison.Ordinal);
        return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0m;
    }

    private static IbOncoPaginatedResponse<T> Paginate<T>(IReadOnlyList<T> rows, int total, int page, int limit, int offset)
    {
        var totalPages = total > 0 ? (int)Math.Ceiling(total / (decimal)limit) : 0;
        return new IbOncoPaginatedResponse<T>(
            rows.Count,
            total,
            page,
            limit,
            offset,
            totalPages,
            page < totalPages,
            page > 1,
            rows);
    }

    private sealed record SaciaUnidad(int Id, string Cluesimb, string Nombre);
    private sealed record SaciaCsvRow(string Clave, decimal Cpm, decimal Existencias);
}

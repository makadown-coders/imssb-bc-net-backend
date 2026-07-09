using Application.Features.Solicitudes;
using Infrastructure.Persistence.Solicitudes;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Solicitudes;

internal sealed class IbOncoService(SolicitudesDbContext dbContext) : IIbOncoService
{
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
        var pagination = NormalizePagination(page, limit, offset);

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
        var pagination = NormalizePagination(page, limit, offset);
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

    private static (int Page, int Limit, int Offset) NormalizePagination(int? page, int? limit, int? offset)
    {
        var normalizedLimit = Math.Clamp(limit ?? 100, 1, 1000);
        var normalizedPage = page.GetValueOrDefault() > 0 ? page!.Value : 1;
        var normalizedOffset = offset.GetValueOrDefault() >= 0
            ? offset!.Value
            : (normalizedPage - 1) * normalizedLimit;

        var effectivePage = (normalizedOffset / normalizedLimit) + 1;
        return (effectivePage, normalizedLimit, normalizedOffset);
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
}

using Application.Features.Solicitudes;
using Domain.Entities.Solicitudes;
using Infrastructure.Persistence.Solicitudes;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Services.Solicitudes;

internal sealed class CatalogoSiciliaService(SolicitudesDbContext dbContext) : ICatalogoSiciliaService
{
    public async Task<CatalogoSiciliaPaginatedResponse<OncoClaseDto>> ListClasesAsync(
        string? q,
        bool? activo,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var pagination = CatalogoSiciliaPagination.Normalize(page, pageSize);
        var search = NormalizeOptional(q);
        var query = dbContext.OncoClases.AsNoTracking()
            .Where(item => !activo.HasValue || item.Activo == activo.Value);

        if (search is not null)
        {
            var pattern = $"%{search}%";
            query = query.Where(item =>
                EF.Functions.ILike(item.Codigo, pattern) ||
                EF.Functions.ILike(item.Nombre, pattern) ||
                (item.Descripcion != null && EF.Functions.ILike(item.Descripcion, pattern)));
        }

        var total = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderBy(item => item.Codigo)
            .Skip(pagination.Offset)
            .Take(pagination.PageSize)
            .Select(item => new OncoClaseDto(
                item.Id,
                item.Codigo,
                item.Nombre,
                item.Descripcion,
                item.StockFactor,
                item.Activo,
                item.CreadoEn,
                item.ActualizadoEn))
            .ToListAsync(cancellationToken);

        return Paginate(rows, total, pagination.Page, pagination.PageSize);
    }

    public Task<OncoClaseDto?> GetClaseAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.OncoClases.AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new OncoClaseDto(
                item.Id,
                item.Codigo,
                item.Nombre,
                item.Descripcion,
                item.StockFactor,
                item.Activo,
                item.CreadoEn,
                item.ActualizadoEn))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<OncoClaseDto> CreateClaseAsync(
        OncoClaseUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var codigo = NormalizeRequired(request.Codigo);
        await EnsureClaseCodeAvailableAsync(codigo, null, cancellationToken);
        var now = DateTime.UtcNow;
        var entity = new OncoClase
        {
            Codigo = codigo,
            Nombre = NormalizeRequired(request.Nombre),
            Descripcion = NormalizeOptional(request.Descripcion),
            StockFactor = request.StockFactor,
            Activo = request.Activo,
            CreadoEn = now,
            ActualizadoEn = now
        };

        dbContext.OncoClases.Add(entity);
        await SaveChangesAsync(codigo, cancellationToken);
        return ToDto(entity);
    }

    public async Task<OncoClaseDto?> UpdateClaseAsync(
        int id,
        OncoClaseUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.OncoClases.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var codigo = NormalizeRequired(request.Codigo);
        await EnsureClaseCodeAvailableAsync(codigo, entity.Id, cancellationToken);
        entity.Codigo = codigo;
        entity.Nombre = NormalizeRequired(request.Nombre);
        entity.Descripcion = NormalizeOptional(request.Descripcion);
        entity.StockFactor = request.StockFactor;
        entity.Activo = request.Activo;
        entity.ActualizadoEn = DateTime.UtcNow;

        await SaveChangesAsync(codigo, cancellationToken);
        return ToDto(entity);
    }

    public async Task<bool> DeactivateClaseAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.OncoClases.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Activo = false;
        entity.ActualizadoEn = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<CatalogoSiciliaPaginatedResponse<OncoSubclaseDto>> ListSubclasesAsync(
        string? q,
        bool? activo,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var pagination = CatalogoSiciliaPagination.Normalize(page, pageSize);
        var search = NormalizeOptional(q);
        var query = dbContext.OncoSubclases.AsNoTracking()
            .Where(item => !activo.HasValue || item.Activo == activo.Value);

        if (search is not null)
        {
            var pattern = $"%{search}%";
            query = query.Where(item =>
                EF.Functions.ILike(item.Codigo, pattern) ||
                EF.Functions.ILike(item.Nombre, pattern) ||
                (item.Descripcion != null && EF.Functions.ILike(item.Descripcion, pattern)));
        }

        var total = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderBy(item => item.Codigo)
            .Skip(pagination.Offset)
            .Take(pagination.PageSize)
            .Select(item => new OncoSubclaseDto(
                item.Id,
                item.Codigo,
                item.Nombre,
                item.Descripcion,
                item.Activo,
                item.CreadoEn,
                item.ActualizadoEn))
            .ToListAsync(cancellationToken);

        return Paginate(rows, total, pagination.Page, pagination.PageSize);
    }

    public Task<OncoSubclaseDto?> GetSubclaseAsync(int id, CancellationToken cancellationToken)
    {
        return dbContext.OncoSubclases.AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new OncoSubclaseDto(
                item.Id,
                item.Codigo,
                item.Nombre,
                item.Descripcion,
                item.Activo,
                item.CreadoEn,
                item.ActualizadoEn))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<OncoSubclaseDto> CreateSubclaseAsync(
        OncoSubclaseUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var codigo = NormalizeRequired(request.Codigo);
        await EnsureSubclaseCodeAvailableAsync(codigo, null, cancellationToken);
        var now = DateTime.UtcNow;
        var entity = new OncoSubclase
        {
            Codigo = codigo,
            Nombre = NormalizeRequired(request.Nombre),
            Descripcion = NormalizeOptional(request.Descripcion),
            Activo = request.Activo,
            CreadoEn = now,
            ActualizadoEn = now
        };

        dbContext.OncoSubclases.Add(entity);
        await SaveChangesAsync(codigo, cancellationToken);
        return ToDto(entity);
    }

    public async Task<OncoSubclaseDto?> UpdateSubclaseAsync(
        int id,
        OncoSubclaseUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var entity = await dbContext.OncoSubclases.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var codigo = NormalizeRequired(request.Codigo);
        await EnsureSubclaseCodeAvailableAsync(codigo, entity.Id, cancellationToken);
        entity.Codigo = codigo;
        entity.Nombre = NormalizeRequired(request.Nombre);
        entity.Descripcion = NormalizeOptional(request.Descripcion);
        entity.Activo = request.Activo;
        entity.ActualizadoEn = DateTime.UtcNow;

        await SaveChangesAsync(codigo, cancellationToken);
        return ToDto(entity);
    }

    public async Task<bool> DeactivateSubclaseAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.OncoSubclases.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.Activo = false;
        entity.ActualizadoEn = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureClaseCodeAvailableAsync(
        string codigo,
        short? excludedId,
        CancellationToken cancellationToken)
    {
        if (await dbContext.OncoClases.AsNoTracking()
            .AnyAsync(item => item.Codigo == codigo && (!excludedId.HasValue || item.Id != excludedId.Value), cancellationToken))
        {
            throw new DuplicateOncoCatalogCodeException(codigo);
        }
    }

    private async Task EnsureSubclaseCodeAvailableAsync(
        string codigo,
        short? excludedId,
        CancellationToken cancellationToken)
    {
        if (await dbContext.OncoSubclases.AsNoTracking()
            .AnyAsync(item => item.Codigo == codigo && (!excludedId.HasValue || item.Id != excludedId.Value), cancellationToken))
        {
            throw new DuplicateOncoCatalogCodeException(codigo);
        }
    }

    private async Task SaveChangesAsync(string codigo, CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new DuplicateOncoCatalogCodeException(codigo);
        }
    }

    private static string NormalizeRequired(string? value) => value!.Trim();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static OncoClaseDto ToDto(OncoClase entity) =>
        new(
            entity.Id,
            entity.Codigo,
            entity.Nombre,
            entity.Descripcion,
            entity.StockFactor,
            entity.Activo,
            entity.CreadoEn,
            entity.ActualizadoEn);

    private static OncoSubclaseDto ToDto(OncoSubclase entity) =>
        new(
            entity.Id,
            entity.Codigo,
            entity.Nombre,
            entity.Descripcion,
            entity.Activo,
            entity.CreadoEn,
            entity.ActualizadoEn);

    private static CatalogoSiciliaPaginatedResponse<T> Paginate<T>(
        IReadOnlyList<T> rows,
        int total,
        int page,
        int pageSize) =>
        new(rows.Count, total, page, pageSize, (int)Math.Ceiling(total / (double)pageSize), rows);
}

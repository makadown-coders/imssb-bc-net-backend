using System.Globalization;
using Application.Features.Solicitudes;
using Domain.Entities.Solicitudes;
using Infrastructure.Persistence.Solicitudes;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Solicitudes;

internal sealed class HomologosCrudService(SolicitudesDbContext dbContext) : IHomologosCrudService
{
    public async Task<HomologoCrudListResponse> ListAsync(CancellationToken cancellationToken)
    {
        var rows = await dbContext.Homologos.AsNoTracking()
            .OrderBy(item => item.Id)
            .Select(MapRow())
            .ToListAsync(cancellationToken);

        return new HomologoCrudListResponse(rows);
    }

    public async Task<HomologoCrudSingleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var row = await dbContext.Homologos.AsNoTracking()
            .Where(item => item.Id == id)
            .Select(MapRow())
            .FirstOrDefaultAsync(cancellationToken);

        return row is null ? null : new HomologoCrudSingleResponse(row);
    }

    public async Task<HomologoCrudSingleResponse> CreateAsync(HomologoCrudUpsertRequest request, CancellationToken cancellationToken)
    {
        var entity = new Homologo
        {
            Clave = NormalizeRequiredKey(request.Clave, "missing_clave"),
            Sustituto = NormalizeRequiredKey(request.Sustituto, "missing_sustituto"),
            Factor = ParseRequiredFactor(request.Factor, "missing_factor", "invalid_factor")
        };

        dbContext.Homologos.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new HomologoCrudSingleResponse(MapRow(entity));
    }

    public async Task<HomologoCrudSingleResponse?> UpdateAsync(int id, HomologoCrudUpsertRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Homologos.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var changed = false;

        if (request.Clave is not null)
        {
            entity.Clave = NormalizeRequiredKey(request.Clave, "invalid_clave");
            changed = true;
        }

        if (request.Sustituto is not null)
        {
            entity.Sustituto = NormalizeRequiredKey(request.Sustituto, "invalid_sustituto");
            changed = true;
        }

        if (request.Factor is not null)
        {
            entity.Factor = ParseRequiredFactor(request.Factor, "invalid_factor", "invalid_factor");
            changed = true;
        }

        if (!changed)
        {
            throw new ArgumentException("no_fields_to_update");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new HomologoCrudSingleResponse(MapRow(entity));
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Homologos.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        dbContext.Homologos.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string NormalizeRequiredKey(string? value, string errorCode)
    {
        var normalized = string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToUpperInvariant();

        return normalized ?? throw new ArgumentException(errorCode);
    }

    private static decimal ParseRequiredFactor(string? value, string missingErrorCode, string invalidErrorCode)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(missingErrorCode);
        }

        if (!decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed))
        {
            throw new ArgumentException(invalidErrorCode);
        }

        return parsed;
    }

    private static HomologoCrudRowDto MapRow(Homologo entity) =>
        new(
            entity.Id,
            entity.Clave.Trim().ToUpperInvariant(),
            entity.Sustituto.Trim().ToUpperInvariant(),
            entity.Factor.ToString(CultureInfo.InvariantCulture));

    private static System.Linq.Expressions.Expression<Func<Homologo, HomologoCrudRowDto>> MapRow() =>
        item => new HomologoCrudRowDto(
            item.Id,
            item.Clave.ToUpper(),
            item.Sustituto.ToUpper(),
            item.Factor.ToString());
}

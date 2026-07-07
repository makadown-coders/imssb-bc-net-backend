using Application.Features.Solicitudes.Catalogos;
using Infrastructure.Persistence.Solicitudes;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Solicitudes;

internal sealed class OperationalCatalogService(SolicitudesDbContext dbContext) : IOperationalCatalogService
{
    public async Task<IReadOnlyList<MunicipioDto>> GetMunicipiosAsync(CancellationToken cancellationToken) =>
        await dbContext.Municipios.AsNoTracking()
            .OrderBy(item => item.NombreMunicipio)
            .Select(item => new MunicipioDto(item.Id, item.NombreMunicipio))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LocalidadDto>> GetLocalidadesAsync(int? municipioId, CancellationToken cancellationToken)
    {
        var query = dbContext.Localidads.AsNoTracking();
        if (municipioId.HasValue) query = query.Where(item => item.MunicipioId == municipioId);

        return await query
            .OrderBy(item => item.Municipio == null ? string.Empty : item.Municipio.NombreMunicipio)
            .ThenBy(item => item.NombreLocalidad)
            .Select(item => new LocalidadDto(
                item.Id,
                item.NombreLocalidad,
                item.MunicipioId,
                municipioId.HasValue || item.Municipio == null ? null : item.Municipio.NombreMunicipio))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TipoUnidadDto>> GetTiposUnidadAsync(CancellationToken cancellationToken) =>
        await dbContext.TipoUnidads.AsNoTracking()
            .OrderBy(item => item.NombreTipo)
            .Select(item => new TipoUnidadDto(item.Id, item.NombreTipo))
            .ToListAsync(cancellationToken);

    public async Task<FactorConversionDto> GetFactorAsync(string clave, CancellationToken cancellationToken)
    {
        var normalized = clave.Trim();
        return await dbContext.FactoresConversions.AsNoTracking()
            .Where(item => item.Clave == normalized)
            .Select(item => new FactorConversionDto(
                item.Clave,
                (item.EnDispensacion ?? 0) != 0,
                item.CantidadFc ?? 1,
                null))
            .FirstOrDefaultAsync(cancellationToken)
            ?? new FactorConversionDto(normalized, false, 1);
    }

    public Task<FactorConversionDto?> GetFactorAsync(string clave, string clues, CancellationToken cancellationToken)
    {
        var normalizedClave = clave.Trim();
        var normalizedClues = clues.Trim();
        return dbContext.FactoresConversions.AsNoTracking()
            .Where(item => item.Clave == normalizedClave && item.Cluesimb == normalizedClues)
            .Select(item => new FactorConversionDto(
                item.Clave,
                (item.EnDispensacion ?? 0) != 0,
                item.CantidadFc ?? 1,
                item.Cluesimb))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FactorConversionListResponse> GetAllFactoresConversionV2Async(CancellationToken cancellationToken)
    {
        var rows = await dbContext.FactoresConversions.AsNoTracking()
            .Where(item => item.Cluesimb != null && item.CantidadFc != null)
            .OrderBy(item => item.Clave)
            .ThenBy(item => item.Cluesimb)
            .Select(item => new FactorConversionLiteDto(
                item.Clave,
                item.Cluesimb!,
                item.CantidadFc ?? 1))
            .ToListAsync(cancellationToken);

        return new FactorConversionListResponse(
            true,
            rows,
            DateTime.UtcNow.ToString("O"),
            "Factores de conversión obtenidos correctamente");
    }
}

namespace Application.Features.Solicitudes.Catalogos;

public interface IOperationalCatalogService
{
    Task<IReadOnlyList<MunicipioDto>> GetMunicipiosAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<LocalidadDto>> GetLocalidadesAsync(int? municipioId, CancellationToken cancellationToken);
    Task<IReadOnlyList<TipoUnidadDto>> GetTiposUnidadAsync(CancellationToken cancellationToken);
    Task<FactorConversionDto> GetFactorAsync(string clave, CancellationToken cancellationToken);
    Task<FactorConversionDto?> GetFactorAsync(string clave, string clues, CancellationToken cancellationToken);
    Task<FactorConversionListResponse> GetAllFactoresConversionV2Async(CancellationToken cancellationToken);
}

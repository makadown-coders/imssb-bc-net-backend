using Application.Features.Solicitudes.Catalogos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Authorize(Policy = "SolicitudesAccess")]
public sealed class OperationalCatalogsController(IOperationalCatalogService service) : ControllerBase
{
    [HttpGet("api/municipios")]
    public Task<IReadOnlyList<MunicipioDto>> GetMunicipios(CancellationToken cancellationToken) =>
        service.GetMunicipiosAsync(cancellationToken);

    [HttpGet("api/localidades")]
    public Task<IReadOnlyList<LocalidadDto>> GetLocalidades(CancellationToken cancellationToken) =>
        service.GetLocalidadesAsync(null, cancellationToken);

    [HttpGet("api/localidades/municipio/{municipioId:int}")]
    public Task<IReadOnlyList<LocalidadDto>> GetLocalidadesByMunicipio(int municipioId, CancellationToken cancellationToken) =>
        service.GetLocalidadesAsync(municipioId, cancellationToken);

    [HttpGet("api/tipo-unidad")]
    public Task<IReadOnlyList<TipoUnidadDto>> GetTiposUnidad(CancellationToken cancellationToken) =>
        service.GetTiposUnidadAsync(cancellationToken);

    [HttpGet("api/factores/{clave}")]
    public Task<FactorConversionDto> GetFactor(string clave, CancellationToken cancellationToken) =>
        service.GetFactorAsync(clave, cancellationToken);

    [HttpGet("api/factores/factor")]
    public async Task<ActionResult<FactorConversionDto?>> GetFactor(
        [FromQuery] string? clave,
        [FromQuery] string? clues,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(clave) || string.IsNullOrWhiteSpace(clues))
        {
            return BadRequest(new { error = "Parámetros requeridos: clave, clues" });
        }

        return Ok(await service.GetFactorAsync(clave, clues, cancellationToken));
    }

    [HttpGet("api/trazabilidad/all-factores-conversion-v2")]
    public Task<FactorConversionListResponse> GetAllFactoresConversionV2(CancellationToken cancellationToken) =>
        service.GetAllFactoresConversionV2Async(cancellationToken);
}

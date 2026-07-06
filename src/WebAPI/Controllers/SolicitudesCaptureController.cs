using Application.Features.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace WebAPI.Controllers;

[ApiController]
[Authorize(Policy = "SolicitudesAccess")]
public sealed class SolicitudesCaptureController(
    ISolicitudesCaptureService service,
    IValidator<CrearBitacoraRequest> validator) : ControllerBase
{
    [HttpGet("api/unidades")]
    [ProducesResponseType<IReadOnlyList<UnidadSolicitudDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<UnidadSolicitudDto>> GetUnidades(CancellationToken cancellationToken) =>
        service.GetUnidadesAsync(cancellationToken);

    [HttpGet("api/articulos")]
    [ProducesResponseType<BuscarArticulosResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BuscarArticulosResponse>> BuscarArticulos(
        [FromQuery] string? q,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 3)
        {
            return BadRequest(new { error = "Query demasiado corta" });
        }

        return Ok(await service.BuscarArticulosAsync(q, cancellationToken));
    }

    [HttpPost("api/solicitudes/bitacora")]
    [ProducesResponseType<CrearBitacoraResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<CrearBitacoraResponse>> CrearBitacora(
        CrearBitacoraRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validation.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray())));
        }

        var result = await service.CrearBitacoraAsync(request, cancellationToken);
        return StatusCode(result.Deduped ? StatusCodes.Status200OK : StatusCodes.Status201Created, result);
    }
}

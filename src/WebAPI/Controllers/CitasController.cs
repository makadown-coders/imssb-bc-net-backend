using Application.Features.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/citas")]
[Authorize(Policy = "SolicitudesAccess")]
public sealed class CitasController(IIbOncoService service) : ControllerBase
{
    [HttpGet("xclave")]
    [ProducesResponseType<IbOncoCitasXClaveResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IbOncoCitasXClaveResponse>> GetXClave(
        [FromQuery] string? clave,
        [FromQuery(Name = "window_days")] int? windowDays,
        [FromQuery(Name = "incluye_no_recibidas")] string? incluyeNoRecibidas,
        [FromQuery] DateOnly? desde,
        [FromQuery] DateOnly? hasta,
        [FromQuery] int? limit,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(clave))
        {
            ModelState.AddModelError("clave", "La clave es requerida.");
            return ValidationProblem(ModelState);
        }

        if (desde.HasValue && hasta.HasValue && desde.Value > hasta.Value)
        {
            ModelState.AddModelError("hasta", "La fecha hasta debe ser igual o posterior a la fecha desde.");
            return ValidationProblem(ModelState);
        }

        var incluirPendientes = !string.Equals(incluyeNoRecibidas, "0", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(incluyeNoRecibidas, "false", StringComparison.OrdinalIgnoreCase);

        return Ok(await service.GetCitasXClaveAsync(
            clave,
            windowDays,
            incluirPendientes,
            desde,
            hasta,
            limit,
            cancellationToken));
    }
}

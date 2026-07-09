using Application.Features.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/ib-onco")]
[Authorize(Policy = "SolicitudesAccess")]
public sealed class IbOncoController(IIbOncoService service) : ControllerBase
{
    [HttpGet("unidades")]
    [ProducesResponseType<IbOncoListResponse<IbOncoUnidadDto>>(StatusCodes.Status200OK)]
    public Task<IbOncoListResponse<IbOncoUnidadDto>> GetUnidades(CancellationToken cancellationToken) =>
        service.GetUnidadesAsync(cancellationToken);

    [HttpGet("claves")]
    [ProducesResponseType<IbOncoListResponse<IbOncoClaveDto>>(StatusCodes.Status200OK)]
    public Task<IbOncoListResponse<IbOncoClaveDto>> GetClaves(
        [FromQuery] string? cluesimb,
        CancellationToken cancellationToken) =>
        service.GetClavesAsync(cluesimb, cancellationToken);

    [HttpGet("abasto-cpm")]
    [ProducesResponseType<IbOncoPaginatedResponse<IbOncoAbastoCpmRowDto>>(StatusCodes.Status200OK)]
    public Task<IbOncoPaginatedResponse<IbOncoAbastoCpmRowDto>> GetAbastoCpm(
        [FromQuery] string? cluesimb,
        [FromQuery(Name = "clave_cnis")] string? claveCnis,
        [FromQuery(Name = "estado_abasto")] string? estadoAbasto,
        [FromQuery] string? search,
        [FromQuery(Name = "window_days")] int? windowDays,
        [FromQuery] int? page,
        [FromQuery] int? limit,
        [FromQuery] int? offset,
        CancellationToken cancellationToken) =>
        service.GetAbastoCpmAsync(
            cluesimb,
            claveCnis,
            estadoAbasto,
            search,
            windowDays,
            page,
            limit,
            offset,
            cancellationToken);

    [HttpGet("citas-pendientes")]
    [ProducesResponseType<IbOncoPaginatedResponse<IbOncoCitaPendienteRowDto>>(StatusCodes.Status200OK)]
    public Task<IbOncoPaginatedResponse<IbOncoCitaPendienteRowDto>> GetCitasPendientes(
        [FromQuery] string? cluesimb,
        [FromQuery(Name = "clave_cnis")] string? claveCnis,
        [FromQuery(Name = "window_days")] int? windowDays,
        [FromQuery] int? page,
        [FromQuery] int? limit,
        [FromQuery] int? offset,
        CancellationToken cancellationToken) =>
        service.GetCitasPendientesAsync(
            cluesimb,
            claveCnis,
            windowDays,
            page,
            limit,
            offset,
            cancellationToken);

    [HttpGet("resumen")]
    [ProducesResponseType<IbOncoListResponse<IbOncoResumenUnidadDto>>(StatusCodes.Status200OK)]
    public Task<IbOncoListResponse<IbOncoResumenUnidadDto>> GetResumen(
        [FromQuery(Name = "window_days")] int? windowDays,
        CancellationToken cancellationToken) =>
        service.GetResumenAsync(windowDays, cancellationToken);
}

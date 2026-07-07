using Application.Features.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using System.Security.Claims;

namespace WebAPI.Controllers;

[ApiController]
[Authorize(Policy = "SolicitudesAccess")]
public sealed class SolicitudesCaptureController(
    ISolicitudesCaptureService service,
    IValidator<CrearBitacoraRequest> validator) : ControllerBase
{
    [HttpGet("api/unidades")]
    [ProducesResponseType<IReadOnlyList<UnidadSolicitudDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<UnidadSolicitudDto>> GetUnidades(
        [FromQuery] string? q,
        [FromQuery] string? nivel,
        CancellationToken cancellationToken) =>
        service.GetUnidadesAsync(q, nivel, cancellationToken);

    [HttpGet("api/unidades/primer-nivel")]
    [ProducesResponseType<IReadOnlyList<UnidadExistenteDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<UnidadExistenteDto>> GetUnidadesPrimerNivel(
        [FromQuery] string? q,
        CancellationToken cancellationToken) =>
        service.GetUnidadesPrimerNivelAsync(q, cancellationToken);

    [HttpGet("api/unidades/todos-niveles")]
    [ProducesResponseType<IReadOnlyList<UnidadExistenteDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<UnidadExistenteDto>> GetUnidadesTodosLosNiveles(
        [FromQuery] string? q,
        CancellationToken cancellationToken) =>
        service.GetUnidadesTodosLosNivelesAsync(q, cancellationToken);

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

    [HttpGet("api/articulos/all")]
    [ProducesResponseType<BuscarArticulosResponse>(StatusCodes.Status200OK)]
    public Task<BuscarArticulosResponse> GetArticulosAll(CancellationToken cancellationToken) =>
        service.GetArticulosAllAsync(cancellationToken);

    [HttpGet("api/articulos/by-cluesimb-cpm")]
    [ProducesResponseType<BuscarArticulosResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<BuscarArticulosResponse>> GetArticulosByCluesimbCpm(
        [FromQuery] string? cluesimb,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.GetArticulosByCluesimbCpmAsync(cluesimb ?? string.Empty, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpGet("api/cpms/expected-vs")]
    [ProducesResponseType<CpmRowsResponse<CpmExpectedVsRowDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CpmRowsResponse<CpmExpectedVsRowDto>>> GetExpectedVsCpm(
        [FromQuery] string? cluesimb,
        [FromQuery] string? cluessa,
        [FromQuery] string? kit,
        [FromQuery] string? clave,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.GetExpectedVsCpmAsync(cluesimb, cluessa, kit, clave, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpGet("api/cpms/by-unidad")]
    [ProducesResponseType<CpmRowsResponse<CpmUnidadRowDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CpmRowsResponse<CpmUnidadRowDto>>> GetCpmByUnidad(
        [FromQuery] string? cluesimb,
        [FromQuery] string? cluessa,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.GetUnidadCpmAsync(cluesimb, cluessa, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpGet("api/cpms/by-unidad-all")]
    [ProducesResponseType<CpmRowsResponse<CpmEditorRowDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CpmRowsResponse<CpmEditorRowDto>>> GetCpmByUnidadAll(
        [FromQuery] string? cluesimb,
        [FromQuery] string? cluessa,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.GetUnidadCpmAllAsync(cluesimb, cluessa, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpGet("api/cpms/by-unidad-real-all")]
    [ProducesResponseType<CpmRowsResponse<CpmEditorRowDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CpmRowsResponse<CpmEditorRowDto>>> GetCpmByUnidadRealAll(
        [FromQuery] string? cluesimb,
        [FromQuery] string? cluessa,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.GetUnidadCpmRealAllAsync(cluesimb, cluessa, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpGet("api/existencias-temp/by-unidad")]
    [ProducesResponseType<ExistenciaRowsResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ExistenciaRowsResponse>> GetExistenciasByUnidad(
        [FromQuery] string? cluesimb,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.GetExistenciasByUnidadAsync(cluesimb ?? string.Empty, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpGet("api/existencias-temp/almacenes-full")]
    [ProducesResponseType<TemporalExistenciaRowsResponse>(StatusCodes.Status200OK)]
    public Task<TemporalExistenciaRowsResponse> GetExistenciasAlmacenesFull(
        CancellationToken cancellationToken) =>
        service.GetExistenciasAlmacenesFullAsync(cancellationToken);

    [HttpGet("api/homologos")]
    [ProducesResponseType<HomologoRowsResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<HomologoRowsResponse>> GetHomologosByClave(
        [FromQuery] string? clave,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.GetHomologosByClaveAsync(clave ?? string.Empty, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost("api/homologos/batch")]
    [ProducesResponseType<HomologoRowsResponse>(StatusCodes.Status200OK)]
    public Task<HomologoRowsResponse> GetHomologosBatch(
        [FromBody] HomologoBatchRequest request,
        CancellationToken cancellationToken) =>
        service.GetHomologosBatchAsync(request.Claves, cancellationToken);

    [HttpPost("api/homologos/batch-forward")]
    [ProducesResponseType<HomologoRowsResponse>(StatusCodes.Status200OK)]
    public Task<HomologoRowsResponse> GetHomologosBatchForward(
        [FromBody] HomologoBatchRequest request,
        CancellationToken cancellationToken) =>
        service.GetHomologosBatchForwardAsync(request.Claves, cancellationToken);

    [HttpGet("api/solicitudes-config/effective")]
    [ProducesResponseType<EffectiveFlagsResponse>(StatusCodes.Status200OK)]
    public Task<EffectiveFlagsResponse> GetEffectiveFlags(
        [FromQuery] string? cluesimb,
        [FromQuery] string? nivel,
        CancellationToken cancellationToken) =>
        service.GetEffectiveFlagsAsync(cluesimb, nivel, cancellationToken);

    [Authorize(Policy = "AdminTic")]
    [HttpGet("api/solicitudes-config")]
    [ProducesResponseType<ListFeatureFlagsResponse>(StatusCodes.Status200OK)]
    public Task<ListFeatureFlagsResponse> ListFeatureFlags(CancellationToken cancellationToken) =>
        service.ListFeatureFlagsAsync(cancellationToken);

    [Authorize(Policy = "AdminTic")]
    [HttpPatch("api/solicitudes-config")]
    [ProducesResponseType<UpsertFeatureFlagsResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UpsertFeatureFlagsResponse>> UpsertFeatureFlags(
        [FromBody] List<UpsertFeatureFlagRequest>? requests,
        CancellationToken cancellationToken)
    {
        var payload = (requests is { Count: > 0 } ? requests : []).ToList();
        if (payload.Count == 0)
        {
            return BadRequest(new { ok = false, error = "flag_key, scope, value son requeridos" });
        }

        try
        {
            var updatedBy = User.FindFirstValue(ClaimTypes.Email)
                ?? User.FindFirstValue(ClaimTypes.Name)
                ?? User.Identity?.Name;
            return Ok(await service.UpsertFeatureFlagsAsync(payload, updatedBy, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { ok = false, error = exception.Message });
        }
    }

    [HttpGet("api/solicitudes-config/allowlist-unidades")]
    [ProducesResponseType<IReadOnlyList<UnidadAllowlistDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<UnidadAllowlistDto>> GetAllowlistUnidades(
        [FromQuery] string? q,
        CancellationToken cancellationToken) =>
        service.GetAllowlistUnidadesAsync(q, cancellationToken);

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

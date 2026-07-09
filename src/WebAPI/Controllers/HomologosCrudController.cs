using Application.Features.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/homologos/crud")]
[Authorize(Policy = "ProyectosSaludAccess")]
public sealed class HomologosCrudController(IHomologosCrudService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<HomologoCrudListResponse>(StatusCodes.Status200OK)]
    public Task<HomologoCrudListResponse> List(CancellationToken cancellationToken) =>
        service.ListAsync(cancellationToken);

    [HttpGet("{id:int}")]
    [ProducesResponseType<HomologoCrudSingleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HomologoCrudSingleResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(new { error = "invalid_id" });
        }

        var row = await service.GetByIdAsync(id, cancellationToken);
        return row is null ? NotFound(new { error = "homologo_not_found" }) : Ok(row);
    }

    [HttpPost]
    [ProducesResponseType<HomologoCrudSingleResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<HomologoCrudSingleResponse>> Create(
        [FromBody] HomologoCrudUpsertRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var row = await service.CreateAsync(request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, row);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<HomologoCrudSingleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HomologoCrudSingleResponse>> Update(
        int id,
        [FromBody] HomologoCrudUpsertRequest request,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(new { error = "invalid_id" });
        }

        try
        {
            var row = await service.UpdateAsync(id, request, cancellationToken);
            return row is null ? NotFound(new { error = "homologo_not_found" }) : Ok(row);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType<HomologoCrudDeleteResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HomologoCrudDeleteResponse>> Delete(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(new { error = "invalid_id" });
        }

        var ok = await service.DeleteAsync(id, cancellationToken);
        return ok ? Ok(new HomologoCrudDeleteResponse(true)) : NotFound(new { error = "homologo_not_found" });
    }
}

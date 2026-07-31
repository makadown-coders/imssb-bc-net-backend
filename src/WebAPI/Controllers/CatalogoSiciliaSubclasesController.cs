using Application.Features.Solicitudes;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Authorize(Policy = "IbOncoAccess")]
[Route("api/ib-onco/catalogo-sicilia/subclases")]
public sealed class CatalogoSiciliaSubclasesController(
    ICatalogoSiciliaService service,
    IValidator<OncoSubclaseUpsertRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CatalogoSiciliaPaginatedResponse<OncoSubclaseDto>>> List(
        [FromQuery] string? q,
        [FromQuery] bool? activo,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        CancellationToken cancellationToken) =>
        Ok(await service.ListSubclasesAsync(q, activo, page, pageSize, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OncoSubclaseDto>> Get(int id, CancellationToken cancellationToken)
    {
        var row = await service.GetSubclaseAsync(id, cancellationToken);
        return row is null ? NotFound() : Ok(row);
    }

    [HttpPost]
    public async Task<ActionResult<OncoSubclaseDto>> Create(
        OncoSubclaseUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var validationProblem = await ValidateAsync(request, cancellationToken);
        if (validationProblem is not null)
        {
            return ValidationProblem(validationProblem);
        }

        try
        {
            var row = await service.CreateSubclaseAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = row.Id }, row);
        }
        catch (DuplicateOncoCatalogCodeException exception)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Código duplicado",
                Detail = exception.Message
            });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OncoSubclaseDto>> Update(
        int id,
        OncoSubclaseUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var validationProblem = await ValidateAsync(request, cancellationToken);
        if (validationProblem is not null)
        {
            return ValidationProblem(validationProblem);
        }

        try
        {
            var row = await service.UpdateSubclaseAsync(id, request, cancellationToken);
            return row is null ? NotFound() : Ok(row);
        }
        catch (DuplicateOncoCatalogCodeException exception)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Código duplicado",
                Detail = exception.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await service.DeactivateSubclaseAsync(id, cancellationToken) ? NoContent() : NotFound();

    private async Task<ValidationProblemDetails?> ValidateAsync(
        OncoSubclaseUpsertRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        return validation.IsValid
            ? null
            : new ValidationProblemDetails(validation.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray()));
    }
}

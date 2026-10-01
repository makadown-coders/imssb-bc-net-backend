using Application.Features.Solicitudes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Authorization;

namespace WebAPI.Controllers;

[ApiController]
[Authorize(Policy = CnisAuthorization.ReadPolicy)]
[Route("api/cnis/grupos-terapeuticos")]
public sealed class CnisGruposTerapeuticosController(ISender sender) : ControllerBase
{
    /// <summary>Obtiene los grupos terapéuticos asociados a artículos existentes.</summary>
    [HttpGet]
    public Task<IReadOnlyList<CnisGrupoTerapeuticoDto>> GetGrupos(CancellationToken cancellationToken) =>
        sender.Send(new GetCnisGruposTerapeuticosQuery(), cancellationToken);

    /// <summary>Obtiene las asociaciones de claves con grupos terapéuticos.</summary>
    [HttpGet("articulos")]
    public Task<IReadOnlyList<CnisArticuloGrupoDto>> GetAsociaciones(CancellationToken cancellationToken) =>
        sender.Send(new GetCnisArticuloGruposQuery(), cancellationToken);

    /// <summary>Obtiene artículos por número de grupo; devuelve una lista vacía si no hay coincidencias.</summary>
    [HttpGet("{numero:int}/articulos")]
    public Task<IReadOnlyList<CnisArticuloDto>> GetArticulos(int numero, CancellationToken cancellationToken) =>
        sender.Send(new GetCnisArticulosPorGrupoQuery(numero), cancellationToken);
}

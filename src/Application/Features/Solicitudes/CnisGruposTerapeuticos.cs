using MediatR;

namespace Application.Features.Solicitudes;

public sealed record CnisGrupoTerapeuticoDto(int Numero, string Nombre);
public sealed record CnisArticuloGrupoDto(string Clave, int Grupo, string GrupoNombre);
public sealed record CnisArticuloDto(string Clave, string? Descripcion, string? Presentacion);

public interface ICnisGruposTerapeuticosService
{
    Task<IReadOnlyList<CnisGrupoTerapeuticoDto>> GetGruposAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<CnisArticuloGrupoDto>> GetAsociacionesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<CnisArticuloDto>> GetArticulosAsync(int numero, CancellationToken cancellationToken);
}

public sealed record GetCnisGruposTerapeuticosQuery : IRequest<IReadOnlyList<CnisGrupoTerapeuticoDto>>;
public sealed record GetCnisArticuloGruposQuery : IRequest<IReadOnlyList<CnisArticuloGrupoDto>>;
public sealed record GetCnisArticulosPorGrupoQuery(int Numero) : IRequest<IReadOnlyList<CnisArticuloDto>>;

public sealed class GetCnisGruposTerapeuticosQueryHandler(ICnisGruposTerapeuticosService service)
    : IRequestHandler<GetCnisGruposTerapeuticosQuery, IReadOnlyList<CnisGrupoTerapeuticoDto>>
{
    public Task<IReadOnlyList<CnisGrupoTerapeuticoDto>> Handle(
        GetCnisGruposTerapeuticosQuery request, CancellationToken cancellationToken) =>
        service.GetGruposAsync(cancellationToken);
}

public sealed class GetCnisArticuloGruposQueryHandler(ICnisGruposTerapeuticosService service)
    : IRequestHandler<GetCnisArticuloGruposQuery, IReadOnlyList<CnisArticuloGrupoDto>>
{
    public Task<IReadOnlyList<CnisArticuloGrupoDto>> Handle(
        GetCnisArticuloGruposQuery request, CancellationToken cancellationToken) =>
        service.GetAsociacionesAsync(cancellationToken);
}

public sealed class GetCnisArticulosPorGrupoQueryHandler(ICnisGruposTerapeuticosService service)
    : IRequestHandler<GetCnisArticulosPorGrupoQuery, IReadOnlyList<CnisArticuloDto>>
{
    public Task<IReadOnlyList<CnisArticuloDto>> Handle(
        GetCnisArticulosPorGrupoQuery request, CancellationToken cancellationToken) =>
        service.GetArticulosAsync(request.Numero, cancellationToken);
}

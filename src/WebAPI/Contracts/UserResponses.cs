namespace WebAPI.Contracts;

public sealed record UserRoleResponse(
    string Code,
    string Descripcion,
    DateTime AssignedAt);

public sealed record ManagedUserResponse(
    Guid Id,
    string Email,
    bool IsActive,
    DateTime CreatedAt,
    int? PersonaId,
    string? NombrePersona,
    int? UnidadId,
    string? NombreUnidad,
    IReadOnlyList<UserRoleResponse> Roles);

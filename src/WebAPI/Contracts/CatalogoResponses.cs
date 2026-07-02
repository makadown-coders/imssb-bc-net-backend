namespace WebAPI.Contracts;

public sealed record TipoUnidadResponse(int Id, string NombreTipo);

public sealed record MunicipioResponse(int Id, string NombreMunicipio);

public sealed record LocalidadResponse(
    int Id,
    string NombreLocalidad,
    int? MunicipioId,
    string? NombreMunicipio);

public sealed record UnidadMedicaResponse(
    int Id,
    string? Cluessa,
    string? Cluesimb,
    string Nombre,
    string? Direccion,
    decimal? Latitud,
    decimal? Longitud,
    string? EstratoUnidad,
    string? NivelAtencion,
    int? TipoUnidadId,
    string? NombreTipoUnidad,
    int? LocalidadId,
    string? NombreLocalidad,
    int? MunicipioId,
    string? NombreMunicipio,
    bool Activo);

public sealed record TipologiaResponse(
    int Id,
    string Nombre,
    bool? EsSegundoNivel);

public sealed record TipologiaUnidadResponse(
    int Id,
    int UnidadMedicaId,
    string NombreUnidadMedica,
    int TipologiaId,
    string NombreTipologia,
    string? Fuente,
    DateTime? CreadoEn);

public sealed record PersonaResponse(
    int Id,
    string NombreCompleto,
    string Nombres,
    string Apellidos,
    string? Cargo,
    int? UnidadMedicaId,
    string? NombreUnidadMedica,
    string? Rfc,
    string? Curp,
    string? CorreoPrincipal,
    string? Username,
    bool Activo,
    DateTime? FechaBaja,
    Guid? UserId,
    string? UserEmail,
    DateTime CreadoEn,
    DateTime ActualizadoEn);

public sealed record UsuarioProvisionadoResponse(
    int PersonaId,
    Guid UserId,
    string Email,
    string RoleCode);

public sealed record RoleResponse(string Code, string Descripcion);

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

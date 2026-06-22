using System.ComponentModel.DataAnnotations;

namespace WebAPI.Contracts;

public sealed record TipoUnidadRequest(
    [Required, MaxLength(100)] string NombreTipo);

public sealed record MunicipioRequest(
    [Required, MaxLength(100)] string NombreMunicipio);

public sealed record LocalidadRequest(
    [Required, MaxLength(150)] string NombreLocalidad,
    int? MunicipioId);

public sealed record UnidadMedicaRequest(
    [MaxLength(20)] string? Cluessa,
    [MaxLength(20)] string? Cluesimb,
    [Required, MaxLength(255)] string Nombre,
    string? Direccion,
    decimal? Latitud,
    decimal? Longitud,
    [MaxLength(10)] string? EstratoUnidad,
    [MaxLength(30)] string? NivelAtencion,
    int? TipoUnidadId,
    int? LocalidadId,
    bool Activo = true);

public sealed record TipologiaRequest(
    [Required] string Nombre,
    bool? EsSegundoNivel);

public sealed record TipologiaUnidadRequest(
    [Required] int UnidadMedicaId,
    [Required] int TipologiaId,
    string? Fuente);

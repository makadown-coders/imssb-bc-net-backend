using System.Text.Json.Serialization;

namespace Application.Features.Solicitudes.Catalogos;

public sealed record MunicipioDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("nombre_municipio")] string NombreMunicipio);

public sealed record LocalidadDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("nombre_localidad")] string NombreLocalidad,
    [property: JsonPropertyName("municipio_id")] int? MunicipioId,
    [property: JsonPropertyName("nombre_municipio")] string? NombreMunicipio = null);

public sealed record TipoUnidadDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("nombre_tipo")] string NombreTipo);

public sealed record FactorConversionDto(
    [property: JsonPropertyName("clave")] string Clave,
    [property: JsonPropertyName("en_dispensacion")] bool EnDispensacion,
    [property: JsonPropertyName("cantidad_fc")] int CantidadFactor,
    [property: JsonPropertyName("cluesimb")] string? Cluesimb = null);

public sealed record FactorConversionLiteDto(
    [property: JsonPropertyName("clave")] string Clave,
    [property: JsonPropertyName("cluesimb")] string Cluesimb,
    [property: JsonPropertyName("factor")] int Factor);

public sealed record FactorConversionListResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("data")] IReadOnlyList<FactorConversionLiteDto> Data,
    [property: JsonPropertyName("timestamp")] string Timestamp,
    [property: JsonPropertyName("message")] string Message);

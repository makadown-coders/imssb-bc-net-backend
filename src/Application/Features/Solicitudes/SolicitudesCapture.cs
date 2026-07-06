using System.Text.Json.Serialization;

namespace Application.Features.Solicitudes;

public sealed record UnidadSolicitudDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("cluessa")] string? Cluessa,
    [property: JsonPropertyName("cluesimb")] string? Cluesimb,
    [property: JsonPropertyName("nombre_municipio")] string? NombreMunicipio,
    [property: JsonPropertyName("nombre_localidad")] string? NombreLocalidad,
    [property: JsonPropertyName("nombre_tipologia")] string? NombreTipologia,
    [property: JsonPropertyName("es_segundo_nivel")] bool? EsSegundoNivel,
    [property: JsonPropertyName("nombre_de_unidad")] string? NombreDeUnidad,
    [property: JsonPropertyName("tipo_unidad")] string? TipoUnidad,
    [property: JsonPropertyName("alias_sas")] string? AliasSas,
    [property: JsonPropertyName("direccion")] string? Direccion,
    [property: JsonPropertyName("latitud")] decimal? Latitud,
    [property: JsonPropertyName("longitud")] decimal? Longitud,
    [property: JsonPropertyName("estrato_unidad")] string? EstratoUnidad,
    [property: JsonPropertyName("nivel_atencion")] string? NivelAtencion);

public sealed record ArticuloSolicitudDto(string Clave, string Descripcion, string Presentacion);
public sealed record BuscarArticulosResponse(IReadOnlyList<ArticuloSolicitudDto> Resultados, int Total);
public sealed record ArticuloBitacoraRequest(string Clave, decimal Cantidad);
public sealed record CrearBitacoraRequest(
    string Cluesimb,
    string TipoPedido,
    string TipoInsumo,
    string? Periodo,
    IReadOnlyList<ArticuloBitacoraRequest> Articulos);
public sealed record CrearBitacoraResponse(bool Ok, Guid SolicitudId, bool Deduped, string PayloadHash);

public interface ISolicitudesCaptureService
{
    Task<IReadOnlyList<UnidadSolicitudDto>> GetUnidadesAsync(CancellationToken cancellationToken);
    Task<BuscarArticulosResponse> BuscarArticulosAsync(string query, CancellationToken cancellationToken);
    Task<CrearBitacoraResponse> CrearBitacoraAsync(CrearBitacoraRequest request, CancellationToken cancellationToken);
}

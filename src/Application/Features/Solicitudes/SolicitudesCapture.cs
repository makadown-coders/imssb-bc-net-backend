using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace Application.Features.Solicitudes;

public sealed record UnidadExistenteDto(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("cluessa")] string? Cluessa,
    [property: JsonPropertyName("cluesimb")] string? Cluesimb,
    [property: JsonPropertyName("nombre")] string Nombre,
    [property: JsonPropertyName("municipio")] string Municipio,
    [property: JsonPropertyName("localidad")] string Localidad,
    [property: JsonPropertyName("jurisdiccion")] string Jurisdiccion,
    [property: JsonPropertyName("direccion")] string Direccion,
    [property: JsonPropertyName("latitud")] string Latitud,
    [property: JsonPropertyName("longitud")] string Longitud,
    [property: JsonPropertyName("estratoUnidad")] string EstratoUnidad,
    [property: JsonPropertyName("nivelAtencion")] string NivelAtencion,
    [property: JsonPropertyName("tipoUnidad")] string TipoUnidad);

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
public sealed record CpmEditorRowDto(
    [property: JsonPropertyName("clave_cnis")] string ClaveCnis,
    [property: JsonPropertyName("cpm")] decimal Cpm,
    [property: JsonPropertyName("fuente")] string? Fuente);
public sealed record CpmExpectedVsRowDto(
    [property: JsonPropertyName("unidad_medica_id")] int? UnidadMedicaId,
    [property: JsonPropertyName("cluesimb")] string? Cluesimb,
    [property: JsonPropertyName("cluessa")] string? Cluessa,
    [property: JsonPropertyName("nombre_unidad")] string? NombreUnidad,
    [property: JsonPropertyName("nombre_tipologia")] string? NombreTipologia,
    [property: JsonPropertyName("kit_codigo")] string? KitCodigo,
    [property: JsonPropertyName("kit_ids")] IReadOnlyList<int>? KitIds,
    [property: JsonPropertyName("kit_codigos")] IReadOnlyList<string>? KitCodigos,
    [property: JsonPropertyName("kit_codigos_txt")] string? KitCodigosTxt,
    [property: JsonPropertyName("clave_cnis")] string? ClaveCnis,
    [property: JsonPropertyName("cpm")] decimal Cpm,
    [property: JsonPropertyName("en_cpm")] bool EnCpm,
    [property: JsonPropertyName("fuentes")] IReadOnlyList<string>? Fuentes);
public sealed record CpmUnidadRowDto(
    [property: JsonPropertyName("clave_cnis")] string ClaveCnis,
    [property: JsonPropertyName("cpm")] decimal Cpm);
public sealed record CpmRowsResponse<T>(
    [property: JsonPropertyName("count")] int Count,
    [property: JsonPropertyName("rows")] IReadOnlyList<T> Rows);
public sealed record HomologoEdgeDto(
    [property: JsonPropertyName("claveConsultada")] string ClaveConsultada,
    [property: JsonPropertyName("candidato")] string Candidato,
    [property: JsonPropertyName("factor")] string Factor,
    [property: JsonPropertyName("direccion")] string Direccion);
public sealed record HomologoRowsResponse(
    [property: JsonPropertyName("rows")] IReadOnlyList<HomologoEdgeDto> Rows);
public sealed record HomologoBatchRequest(
    [property: JsonPropertyName("claves")] IReadOnlyList<string> Claves);
public sealed record ExistenciaUnidadRowDto(
    [property: JsonPropertyName("clave_cnis")] string ClaveCnis,
    [property: JsonPropertyName("descripcion")] string Descripcion,
    [property: JsonPropertyName("existencia_total")] decimal ExistenciaTotal);
public sealed record ExistenciaRowsResponse(
    [property: JsonPropertyName("rows")] IReadOnlyList<ExistenciaUnidadRowDto> Rows);
public sealed record TemporalExistenciaRowDto(
    [property: JsonPropertyName("fuente")] string Fuente,
    [property: JsonPropertyName("alias_sas")] string? AliasSas,
    [property: JsonPropertyName("cluessa")] string? Cluessa,
    [property: JsonPropertyName("cluesimb")] string? Cluesimb,
    [property: JsonPropertyName("clave_cnis")] string ClaveCnis,
    [property: JsonPropertyName("lote")] string? Lote,
    [property: JsonPropertyName("fecha_caducidad")] DateOnly? FechaCaducidad,
    [property: JsonPropertyName("existencia")] decimal Existencia);
public sealed record TemporalExistenciaRowsResponse(
    [property: JsonPropertyName("count")] int Count,
    [property: JsonPropertyName("rows")] IReadOnlyList<TemporalExistenciaRowDto> Rows);
public sealed record EffectiveFlagsResponse(
    [property: JsonPropertyName("ok")] bool Ok,
    [property: JsonPropertyName("flags")] IReadOnlyDictionary<string, object?> Flags);
public sealed record FeatureFlagRowDto(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("flag_key")] string FlagKey,
    [property: JsonPropertyName("scope")] string Scope,
    [property: JsonPropertyName("scope_id")] string? ScopeId,
    [property: JsonPropertyName("value_json")] JsonNode? ValueJson,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("updated_by")] string? UpdatedBy,
    [property: JsonPropertyName("updated_at")] DateTime UpdatedAt);
public sealed record ListFeatureFlagsResponse(
    [property: JsonPropertyName("ok")] bool Ok,
    [property: JsonPropertyName("rows")] IReadOnlyList<FeatureFlagRowDto> Rows);
public sealed record UpsertFeatureFlagRequest(
    [property: JsonPropertyName("flag_key")] string FlagKey,
    [property: JsonPropertyName("scope")] string Scope,
    [property: JsonPropertyName("scope_id")] string? ScopeId,
    [property: JsonPropertyName("value")] JsonNode? Value);
public sealed record UpsertFeatureFlagsResponse(
    [property: JsonPropertyName("ok")] bool Ok,
    [property: JsonPropertyName("updated")] int Updated,
    [property: JsonPropertyName("rows")] IReadOnlyList<FeatureFlagRowDto> Rows);
public sealed record UnidadAllowlistDto(
    [property: JsonPropertyName("cluesimb")] string Cluesimb,
    [property: JsonPropertyName("nombre")] string Nombre,
    [property: JsonPropertyName("alias_dash")] string AliasDash);
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
    Task<IReadOnlyList<UnidadSolicitudDto>> GetUnidadesAsync(string? query, string? nivel, CancellationToken cancellationToken);
    Task<IReadOnlyList<UnidadExistenteDto>> GetUnidadesPrimerNivelAsync(string? query, CancellationToken cancellationToken);
    Task<IReadOnlyList<UnidadExistenteDto>> GetUnidadesTodosLosNivelesAsync(string? query, CancellationToken cancellationToken);
    Task<BuscarArticulosResponse> BuscarArticulosAsync(string query, CancellationToken cancellationToken);
    Task<BuscarArticulosResponse> GetArticulosAllAsync(CancellationToken cancellationToken);
    Task<BuscarArticulosResponse> GetArticulosByCluesimbCpmAsync(string cluesimb, CancellationToken cancellationToken);
    Task<CpmRowsResponse<CpmExpectedVsRowDto>> GetExpectedVsCpmAsync(string? cluesimb, string? cluessa, string? kit, string? clave, CancellationToken cancellationToken);
    Task<CpmRowsResponse<CpmUnidadRowDto>> GetUnidadCpmAsync(string? cluesimb, string? cluessa, CancellationToken cancellationToken);
    Task<CpmRowsResponse<CpmEditorRowDto>> GetUnidadCpmAllAsync(string? cluesimb, string? cluessa, CancellationToken cancellationToken);
    Task<CpmRowsResponse<CpmEditorRowDto>> GetUnidadCpmRealAllAsync(string? cluesimb, string? cluessa, CancellationToken cancellationToken);
    Task<ExistenciaRowsResponse> GetExistenciasByUnidadAsync(string cluesimb, CancellationToken cancellationToken);
    Task<TemporalExistenciaRowsResponse> GetExistenciasAlmacenesFullAsync(CancellationToken cancellationToken);
    Task<HomologoRowsResponse> GetHomologosByClaveAsync(string clave, CancellationToken cancellationToken);
    Task<HomologoRowsResponse> GetHomologosBatchAsync(IReadOnlyList<string> claves, CancellationToken cancellationToken);
    Task<HomologoRowsResponse> GetHomologosBatchForwardAsync(IReadOnlyList<string> claves, CancellationToken cancellationToken);
    Task<EffectiveFlagsResponse> GetEffectiveFlagsAsync(string? cluesimb, string? nivel, CancellationToken cancellationToken);
    Task<ListFeatureFlagsResponse> ListFeatureFlagsAsync(CancellationToken cancellationToken);
    Task<UpsertFeatureFlagsResponse> UpsertFeatureFlagsAsync(IReadOnlyList<UpsertFeatureFlagRequest> requests, string? updatedBy, CancellationToken cancellationToken);
    Task<IReadOnlyList<UnidadAllowlistDto>> GetAllowlistUnidadesAsync(string? query, CancellationToken cancellationToken);
    Task<CrearBitacoraResponse> CrearBitacoraAsync(CrearBitacoraRequest request, CancellationToken cancellationToken);
}

using System.Text.Json.Serialization;

namespace Application.Features.Solicitudes;

public sealed record IbOncoUnidadDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("cluesimb")] string Cluesimb,
    [property: JsonPropertyName("cluessa")] string? Cluessa,
    [property: JsonPropertyName("nombre_de_unidad")] string? NombreDeUnidad,
    [property: JsonPropertyName("nombre_municipio")] string? NombreMunicipio);

public sealed record IbOncoClaveDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("cluesimb")] string Cluesimb,
    [property: JsonPropertyName("clave_cnis")] string ClaveCnis,
    [property: JsonPropertyName("descripcion")] string? Descripcion);

public sealed record IbOncoAbastoCpmRowDto(
    [property: JsonPropertyName("cluesimb")] string Cluesimb,
    [property: JsonPropertyName("nombre_de_unidad")] string? NombreDeUnidad,
    [property: JsonPropertyName("clave_cnis")] string ClaveCnis,
    [property: JsonPropertyName("descripcion")] string? Descripcion,
    [property: JsonPropertyName("existencias")] decimal Existencias,
    [property: JsonPropertyName("cpm")] decimal Cpm,
    [property: JsonPropertyName("cpm_x_3")] decimal CpmX3,
    [property: JsonPropertyName("cpms_eq")] decimal CpmsEq,
    [property: JsonPropertyName("estado_abasto")] string? EstadoAbasto,
    [property: JsonPropertyName("citas_pendientes")] int CitasPendientes,
    [property: JsonPropertyName("piezas_pendientes")] decimal PiezasPendientes,
    [property: JsonPropertyName("tiene_citas_pendientes")] bool TieneCitasPendientes);

public sealed record IbOncoCitaPendienteRowDto(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("ejercicio")] int? Ejercicio,
    [property: JsonPropertyName("orden_de_suministro")] string? OrdenDeSuministro,
    [property: JsonPropertyName("institucion")] string? Institucion,
    [property: JsonPropertyName("contrato")] string? Contrato,
    [property: JsonPropertyName("cluesimb")] string Cluesimb,
    [property: JsonPropertyName("nombre_de_unidad")] string? NombreDeUnidad,
    [property: JsonPropertyName("clave_cnis")] string ClaveCnis,
    [property: JsonPropertyName("descripcion")] string? Descripcion,
    [property: JsonPropertyName("proveedor")] string? Proveedor,
    [property: JsonPropertyName("compra")] string? Compra,
    [property: JsonPropertyName("tipo_de_entrega")] string? TipoDeEntrega,
    [property: JsonPropertyName("fte_fmto")] string? FteFmto,
    [property: JsonPropertyName("tipo_de_red")] string? TipoDeRed,
    [property: JsonPropertyName("tipo_de_insumo")] string? TipoDeInsumo,
    [property: JsonPropertyName("grupo_terapeutico")] string? GrupoTerapeutico,
    [property: JsonPropertyName("precio_unitario")] decimal PrecioUnitario,
    [property: JsonPropertyName("no_de_piezas_emitidas")] int NoDePiezasEmitidas,
    [property: JsonPropertyName("pzas_recibidas_por_la_entidad")] decimal PzasRecibidasPorLaEntidad,
    [property: JsonPropertyName("fecha_emision")] DateOnly? FechaEmision,
    [property: JsonPropertyName("fecha_limite_de_entrega")] DateOnly? FechaLimiteDeEntrega,
    [property: JsonPropertyName("fecha_de_cita")] DateOnly? FechaDeCita,
    [property: JsonPropertyName("estatus")] string? Estatus,
    [property: JsonPropertyName("folio_abasto")] string? FolioAbasto);

public sealed record IbOncoResumenUnidadDto(
    [property: JsonPropertyName("cluesimb")] string Cluesimb,
    [property: JsonPropertyName("nombre_de_unidad")] string? NombreDeUnidad,
    [property: JsonPropertyName("claves_onco")] int ClavesOnco,
    [property: JsonPropertyName("claves_posible_sobre_abasto")] int ClavesPosibleSobreAbasto,
    [property: JsonPropertyName("existencias_total")] decimal ExistenciasTotal,
    [property: JsonPropertyName("cpm_total")] decimal CpmTotal,
    [property: JsonPropertyName("citas_pendientes")] int CitasPendientes,
    [property: JsonPropertyName("piezas_pendientes")] decimal PiezasPendientes);

public sealed record IbOncoCitaXClaveDto(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("ejercicio")] int? Ejercicio,
    [property: JsonPropertyName("orden_de_suministro")] string? OrdenDeSuministro,
    [property: JsonPropertyName("procedimiento")] string? Procedimiento,
    [property: JsonPropertyName("tipo_de_entrega")] string? TipoDeEntrega,
    [property: JsonPropertyName("unidad")] string? Unidad,
    [property: JsonPropertyName("fte_fmto")] string? FteFmto,
    [property: JsonPropertyName("compra")] string? Compra,
    [property: JsonPropertyName("no_de_piezas_emitidas")] int NoDePiezasEmitidas,
    [property: JsonPropertyName("pzas_recibidas_por_la_entidad")] decimal PzasRecibidasPorLaEntidad,
    [property: JsonPropertyName("fecha_emision")] DateOnly? FechaEmision,
    [property: JsonPropertyName("fecha_recepcion_lista")] IReadOnlyList<DateOnly>? FechaRecepcionLista,
    [property: JsonPropertyName("fecha_limite_de_entrega")] DateOnly? FechaLimiteDeEntrega,
    [property: JsonPropertyName("fecha_de_cita")] DateOnly? FechaDeCita,
    [property: JsonPropertyName("estatus")] string? Estatus,
    [property: JsonPropertyName("contrato")] string? Contrato,
    [property: JsonPropertyName("grupo_terapeutico")] string? GrupoTerapeutico,
    [property: JsonPropertyName("tipo_de_red")] string? TipoDeRed,
    [property: JsonPropertyName("tipo_de_insumo")] string? TipoDeInsumo,
    [property: JsonPropertyName("proveedor")] string? Proveedor);

public sealed record IbOncoCitasXClaveResponse(
    [property: JsonPropertyName("ok")] bool Ok,
    [property: JsonPropertyName("rows")] IReadOnlyList<IbOncoCitaXClaveDto> Rows,
    [property: JsonPropertyName("ref")] IbOncoCitaXClaveDto? Ref);

public sealed record IbOncoSaciaUnidadResult(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("cluesimb")] string Cluesimb,
    [property: JsonPropertyName("unidad")] string Unidad,
    [property: JsonPropertyName("ok")] bool Ok,
    [property: JsonPropertyName("leidos")] int Leidos,
    [property: JsonPropertyName("claves")] int Claves,
    [property: JsonPropertyName("onco_claves_insertados")] int OncoClavesInsertados,
    [property: JsonPropertyName("tmp_existencias_eliminados")] int TmpExistenciasEliminados,
    [property: JsonPropertyName("tmp_existencias_insertados")] int TmpExistenciasInsertados,
    [property: JsonPropertyName("error")] string? Error = null);

public sealed record IbOncoSaciaUpdateResponse(
    [property: JsonPropertyName("ok")] bool Ok,
    [property: JsonPropertyName("fuente")] string Fuente,
    [property: JsonPropertyName("started_at")] DateTime StartedAt,
    [property: JsonPropertyName("finished_at")] DateTime FinishedAt,
    [property: JsonPropertyName("unidades_total")] int UnidadesTotal,
    [property: JsonPropertyName("unidades_ok")] int UnidadesOk,
    [property: JsonPropertyName("unidades_error")] int UnidadesError,
    [property: JsonPropertyName("claves_insertadas")] int ClavesInsertadas,
    [property: JsonPropertyName("existencias_eliminadas")] int ExistenciasEliminadas,
    [property: JsonPropertyName("existencias_insertadas")] int ExistenciasInsertadas,
    [property: JsonPropertyName("unidades")] IReadOnlyList<IbOncoSaciaUnidadResult> Unidades);

public sealed record IbOncoListResponse<T>(
    [property: JsonPropertyName("ok")] bool Ok,
    [property: JsonPropertyName("count")] int Count,
    [property: JsonPropertyName("data")] IReadOnlyList<T> Data);

public sealed record IbOncoPaginatedResponse<T>(
    [property: JsonPropertyName("count")] int Count,
    [property: JsonPropertyName("total")] int Total,
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("limit")] int Limit,
    [property: JsonPropertyName("offset")] int Offset,
    [property: JsonPropertyName("totalPages")] int TotalPages,
    [property: JsonPropertyName("hasNextPage")] bool HasNextPage,
    [property: JsonPropertyName("hasPrevPage")] bool HasPrevPage,
    [property: JsonPropertyName("rows")] IReadOnlyList<T> Rows);

public interface IIbOncoService
{
    Task<IbOncoListResponse<IbOncoUnidadDto>> GetUnidadesAsync(CancellationToken cancellationToken);
    Task<IbOncoListResponse<IbOncoClaveDto>> GetClavesAsync(string? cluesimb, CancellationToken cancellationToken);
    Task<IbOncoPaginatedResponse<IbOncoAbastoCpmRowDto>> GetAbastoCpmAsync(
        string? cluesimb,
        string? claveCnis,
        string? estadoAbasto,
        string? search,
        int? windowDays,
        int? page,
        int? limit,
        int? offset,
        CancellationToken cancellationToken);
    Task<IbOncoPaginatedResponse<IbOncoCitaPendienteRowDto>> GetCitasPendientesAsync(
        string? cluesimb,
        string? claveCnis,
        int? windowDays,
        int? page,
        int? limit,
        int? offset,
        CancellationToken cancellationToken);
    Task<IbOncoListResponse<IbOncoResumenUnidadDto>> GetResumenAsync(int? windowDays, CancellationToken cancellationToken);
    Task<IbOncoCitasXClaveResponse> GetCitasXClaveAsync(
        string? clave,
        int? windowDays,
        bool incluyeNoRecibidas,
        DateOnly? desde,
        DateOnly? hasta,
        int? limit,
        CancellationToken cancellationToken);
    Task<IbOncoSaciaUpdateResponse> UpdateSaciaAsync(CancellationToken cancellationToken);
}

public static class IbOncoPagination
{
    public static (int Page, int Limit, int Offset) Normalize(int? page, int? limit, int? offset)
    {
        var normalizedLimit = Math.Clamp(limit ?? 100, 1, 1000);
        var normalizedPage = page.GetValueOrDefault() > 0 ? page!.Value : 1;
        var normalizedOffset = offset.HasValue && offset.Value >= 0
            ? offset.Value
            : (normalizedPage - 1) * normalizedLimit;

        var effectivePage = (normalizedOffset / normalizedLimit) + 1;
        return (effectivePage, normalizedLimit, normalizedOffset);
    }
}

using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Cita
{
    public int? Ejercicio { get; set; }

    public string? OrdenDeSuministro { get; set; }

    public string? Institucion { get; set; }

    public string? Contrato { get; set; }

    public string? TipoDeEntrega { get; set; }

    public string? CluesDestino { get; set; }

    public string? Unidad { get; set; }

    public string? FteFmto { get; set; }

    public string? Proveedor { get; set; }

    public string? ClaveCnis { get; set; }

    public string? Descripcion { get; set; }

    public string? Compra { get; set; }

    public string? TipoDeRed { get; set; }

    public string? TipoDeInsumo { get; set; }

    public string? GrupoTerapeutico { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public int? NoDePiezasEmitidas { get; set; }

    public DateOnly? FechaEmision { get; set; }

    public DateOnly? FechaLimiteDeEntrega { get; set; }

    public decimal? PzasRecibidasPorLaEntidad { get; set; }

    public string? FechaRecepcionAlmacen { get; set; }

    public string? NumeroDeRemision { get; set; }

    public string? Lote { get; set; }

    public string? Caducidad { get; set; }

    public string? Estatus { get; set; }

    public string? FolioAbasto { get; set; }

    public string? AlmacenHospitalQueRecibio { get; set; }

    public string? Evidencia { get; set; }

    public string? Carga { get; set; }

    public DateOnly? FechaDeCita { get; set; }

    public long Id { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? Recibido { get; set; }

    public List<DateOnly>? FechaRecepcionLista { get; set; }

    public DateOnly? FechaRecepcionMin { get; set; }

    public DateOnly? FechaRecepcionMax { get; set; }

    public int? AtrasoDias { get; set; }

    public string? Procedimiento { get; set; }
}

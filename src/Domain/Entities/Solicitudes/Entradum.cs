using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Entradum
{
    public int Id { get; set; }

    public int? UnidadDestinoId { get; set; }

    public string? UnidadDestinoTexto { get; set; }

    public string? ClaveCnis { get; set; }

    public string? Descripcion { get; set; }

    public string? NumFactura { get; set; }

    public string? Folio { get; set; }

    public string? Proveedor { get; set; }

    public decimal? Cantidad { get; set; }

    public decimal? Costo { get; set; }

    public DateOnly? Fecha { get; set; }

    public short? TipoDocumento { get; set; }

    public string? NumRemision { get; set; }

    public string? Observaciones { get; set; }

    public short? Anio { get; set; }

    public string? Lote { get; set; }

    public DateOnly? FechaCaducidad { get; set; }

    public decimal? CantidadExistencia { get; set; }

    public string? DescripcionExtra { get; set; }

    public DateTime? CreadoEn { get; set; }

    public virtual UnidadMedicaAlias? UnidadDestino { get; set; }
}

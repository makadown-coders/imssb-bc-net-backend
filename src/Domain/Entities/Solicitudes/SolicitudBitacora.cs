using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class SolicitudBitacora
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateOnly? CreatedDay { get; set; }

    public string Cluesimb { get; set; } = null!;

    public string TipoPedido { get; set; } = null!;

    public List<string> TiposInsumo { get; set; } = null!;

    public string? PeriodoTexto { get; set; }

    public string ExportKind { get; set; } = null!;

    public int TotalRenglones { get; set; }

    public decimal TotalPiezas { get; set; }

    public string PayloadHash { get; set; } = null!;

    public virtual ICollection<SolicitudBitacoraDetalle> SolicitudBitacoraDetalles { get; set; } = new List<SolicitudBitacoraDetalle>();
}

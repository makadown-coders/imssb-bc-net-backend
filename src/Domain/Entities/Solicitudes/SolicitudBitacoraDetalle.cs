using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class SolicitudBitacoraDetalle
{
    public long Id { get; set; }

    public Guid SolicitudId { get; set; }

    public string Clave { get; set; } = null!;

    public string? UnidadMedida { get; set; }

    public decimal Cantidad { get; set; }

    public virtual SolicitudBitacora Solicitud { get; set; } = null!;
}

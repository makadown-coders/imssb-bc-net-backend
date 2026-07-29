using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class PermResumenAlmacenesFinal
{
    public int Id { get; set; }

    public int EjecucionId { get; set; }

    public DateTime? FechaEjecucion { get; set; }

    public string ClaveCnis { get; set; } = null!;

    public string JurisdiccionAlmacen { get; set; } = null!;

    public string? JurisdiccionDestino { get; set; }

    public int? TotalUnidades { get; set; }

    public int? TotalPiezas { get; set; }

    public string? InstruccionesDetalladas { get; set; }

    public virtual LogEjecucionesBalanceo Ejecucion { get; set; } = null!;
}

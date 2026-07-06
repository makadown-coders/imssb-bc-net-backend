using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class LogEjecucionesBalanceo
{
    public int Id { get; set; }

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public int? TotalClaves { get; set; }

    public int? ClavesProcesadas { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<PermBalanceoDetalladoFinal> PermBalanceoDetalladoFinals { get; set; } = new List<PermBalanceoDetalladoFinal>();

    public virtual ICollection<PermBalanceoResultado> PermBalanceoResultados { get; set; } = new List<PermBalanceoResultado>();

    public virtual ICollection<PermResumenAlmacenesFinal> PermResumenAlmacenesFinals { get; set; } = new List<PermResumenAlmacenesFinal>();
}

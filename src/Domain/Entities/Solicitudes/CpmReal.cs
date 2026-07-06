using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class CpmReal
{
    public long Id { get; set; }

    public string Cluesimb { get; set; } = null!;

    public string ClaveCnis { get; set; } = null!;

    public decimal Cpm { get; set; }

    public int? UnidadOncologica55 { get; set; }

    public int? ClavesDeKit { get; set; }

    public int? TemporalidadDeEntrega { get; set; }
}

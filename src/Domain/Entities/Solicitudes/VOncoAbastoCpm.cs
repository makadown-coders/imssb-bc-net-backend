using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class VOncoAbastoCpm
{
    public string? Cluesimb { get; set; }

    public string? NombreDeUnidad { get; set; }

    public string? ClaveCnis { get; set; }

    public decimal? Existencias { get; set; }

    public decimal? Cpm { get; set; }

    public decimal? CpmX3 { get; set; }

    public decimal? CpmsEq { get; set; }

    public string? EstadoAbasto { get; set; }
}

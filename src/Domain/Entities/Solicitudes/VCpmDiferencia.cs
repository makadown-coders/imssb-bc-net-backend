using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class VCpmDiferencia
{
    public string? Cluesimb { get; set; }

    public string? NombreDeUnidad { get; set; }

    public string? ClaveCnis { get; set; }

    public decimal? CpmCdmx { get; set; }

    public decimal? CpmPropuesto { get; set; }

    public decimal? Diferencia { get; set; }

    public string? Observacion { get; set; }
}

using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class VUnidadCpm
{
    public int? UnidadMedicaId { get; set; }

    public string? Cluesimb { get; set; }

    public string? NombreUnidad { get; set; }

    public string? ClaveCnis { get; set; }

    public decimal? Cpm { get; set; }
}

using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class VExistenciasConsolidada
{
    public string? ClaveCnis { get; set; }

    public string? Descripcion { get; set; }

    public string? Cluessa { get; set; }

    public string? Cluesimb { get; set; }

    public string? NombreDeUnidad { get; set; }

    public string? NombreMunicipio { get; set; }

    public decimal? Existencia { get; set; }
}

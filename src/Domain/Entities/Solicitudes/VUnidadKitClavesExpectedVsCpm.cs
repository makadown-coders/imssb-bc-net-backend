using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class VUnidadKitClavesExpectedVsCpm
{
    public int? UnidadMedicaId { get; set; }

    public string? Cluesimb { get; set; }

    public string? Cluessa { get; set; }

    public string? NombreUnidad { get; set; }

    public string? NombreTipologia { get; set; }

    public string? KitCodigo { get; set; }

    public List<long>? KitIds { get; set; }

    public List<string>? KitCodigos { get; set; }

    public string? KitCodigosTxt { get; set; }

    public string? ClaveCnis { get; set; }

    public decimal? Cpm { get; set; }

    public bool? EnCpm { get; set; }

    public List<string>? Fuentes { get; set; }
}

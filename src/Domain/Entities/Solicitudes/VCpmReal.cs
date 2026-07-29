using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class VCpmReal
{
    public long? Id { get; set; }

    public int? UnidadMedicaId { get; set; }

    public string? Cluesimb { get; set; }

    public string? Cluessa { get; set; }

    public string? ClaveCnis { get; set; }

    public decimal? Cpm { get; set; }

    public string? Fuente { get; set; }

    public string? FuenteRaw { get; set; }

    public DateTime? CreadoEn { get; set; }
}

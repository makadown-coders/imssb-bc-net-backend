using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Cpm
{
    public long Id { get; set; }

    public int UnidadMedicaId { get; set; }

    public string ClaveCnis { get; set; } = null!;

    public decimal Cpm1 { get; set; }

    public string? Fuente { get; set; }

    public DateTime? CreadoEn { get; set; }

    public virtual UnidadMedica UnidadMedica { get; set; } = null!;
}

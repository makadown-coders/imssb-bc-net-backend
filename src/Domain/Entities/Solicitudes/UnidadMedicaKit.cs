using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class UnidadMedicaKit
{
    public int Id { get; set; }

    public int UnidadMedicaId { get; set; }

    public int KitId { get; set; }

    public string? Fuente { get; set; }

    public DateTime CreadoEn { get; set; }

    public virtual Kit Kit { get; set; } = null!;

    public virtual UnidadMedica UnidadMedica { get; set; } = null!;
}

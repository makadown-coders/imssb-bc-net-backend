using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class EstadoDispositivo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<AsignacionDispositivo> AsignacionDispositivos { get; set; } = new List<AsignacionDispositivo>();
}

using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class KitClave
{
    public int KitId { get; set; }

    public string Clave { get; set; } = null!;

    public bool Aplica { get; set; }

    public int Id { get; set; }

    public virtual Kit Kit { get; set; } = null!;
}

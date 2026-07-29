using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class TipoDispositivo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Dispositivo> Dispositivos { get; set; } = new List<Dispositivo>();
}

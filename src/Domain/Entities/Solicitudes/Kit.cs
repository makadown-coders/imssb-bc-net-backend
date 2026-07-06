using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Kit
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Nombre { get; set; }

    public virtual ICollection<KitClave> KitClaves { get; set; } = new List<KitClave>();

    public virtual ICollection<UnidadMedicaKit> UnidadMedicaKits { get; set; } = new List<UnidadMedicaKit>();
}

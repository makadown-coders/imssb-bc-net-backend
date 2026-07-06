using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class UnidadMedicaAlias
{
    public int Id { get; set; }

    public int UnidadMedicaId { get; set; }

    public string? AliasSas { get; set; }

    public string? AliasDash { get; set; }

    public DateTime? CreadoEn { get; set; }

    public DateTime? ActualizadoEn { get; set; }

    public virtual ICollection<Entradum> Entrada { get; set; } = new List<Entradum>();

    public virtual ICollection<InventarioInicial> InventarioInicials { get; set; } = new List<InventarioInicial>();

    public virtual ICollection<Salidum> SalidumUnidadDestinos { get; set; } = new List<Salidum>();

    public virtual ICollection<Salidum> SalidumUnidadOrigens { get; set; } = new List<Salidum>();

    public virtual ICollection<Traspaso> TraspasoUnidadDestinos { get; set; } = new List<Traspaso>();

    public virtual ICollection<Traspaso> TraspasoUnidadOrigens { get; set; } = new List<Traspaso>();

    public virtual UnidadMedica UnidadMedica { get; set; } = null!;
}

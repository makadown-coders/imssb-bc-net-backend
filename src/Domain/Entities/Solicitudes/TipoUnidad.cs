using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class TipoUnidad
{
    public int Id { get; set; }

    public string NombreTipo { get; set; } = null!;

    public virtual ICollection<UnidadMedica> UnidadMedicas { get; set; } = new List<UnidadMedica>();
}

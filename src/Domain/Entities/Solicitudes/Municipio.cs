using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Municipio
{
    public int Id { get; set; }

    public string NombreMunicipio { get; set; } = null!;

    public virtual ICollection<Localidad> Localidads { get; set; } = new List<Localidad>();
}

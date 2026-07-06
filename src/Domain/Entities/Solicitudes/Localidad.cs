using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Localidad
{
    public int Id { get; set; }

    public string NombreLocalidad { get; set; } = null!;

    public int? MunicipioId { get; set; }

    public virtual Municipio? Municipio { get; set; }

    public virtual ICollection<UnidadMedica> UnidadMedicas { get; set; } = new List<UnidadMedica>();
}

using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class PersonaCorreo
{
    public int Id { get; set; }

    public int PersonaId { get; set; }

    public string Correo { get; set; } = null!;

    public bool? EsPrincipal { get; set; }

    public bool Activo { get; set; }

    public virtual Persona Persona { get; set; } = null!;
}

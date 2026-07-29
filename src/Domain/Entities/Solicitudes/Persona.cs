using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Persona
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string? Cargo { get; set; }

    public int? UnidadMedicaId { get; set; }

    public DateTime? CreadoEn { get; set; }

    public DateTime? ActualizadoEn { get; set; }

    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public string? Rfc { get; set; }

    public string? Curp { get; set; }

    public string? CorreoPrincipal { get; set; }

    public string? Username { get; set; }

    public bool Activo { get; set; }

    public DateTime? FechaBaja { get; set; }

    public Guid? UserId { get; set; }

    public virtual ICollection<AsignacionDispositivo> AsignacionDispositivos { get; set; } = new List<AsignacionDispositivo>();

    public virtual PersonaCorreo? PersonaCorreo { get; set; }

    public virtual UnidadMedica? UnidadMedica { get; set; }
}

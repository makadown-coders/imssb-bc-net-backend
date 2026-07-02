namespace Domain.Entities;

public sealed class Persona
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Cargo { get; set; }
    public int? UnidadMedicaId { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;
    public string? Nombres { get; set; }
    public string? Apellidos { get; set; }
    public string? Rfc { get; set; }
    public string? Curp { get; set; }
    public string? CorreoPrincipal { get; set; }
    public string? Username { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime? FechaBaja { get; set; }
    public Guid? UserId { get; set; }
    public UnidadMedica? UnidadMedica { get; set; }
    public User? User { get; set; }
}

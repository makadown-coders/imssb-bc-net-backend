using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class RadarEvento
{
    public int Id { get; set; }

    public DateOnly FechaEvento { get; set; }

    public string Clues { get; set; } = null!;

    public string? UnidadNombre { get; set; }

    public string? TipoInsumo { get; set; }

    public DateOnly? FechaReferencia { get; set; }

    public string Motivo { get; set; } = null!;

    public string? Observaciones { get; set; }

    public string Estado { get; set; } = null!;

    public string? CreadoPor { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<RadarEventoClafe> RadarEventoClaves { get; set; } = new List<RadarEventoClafe>();
}

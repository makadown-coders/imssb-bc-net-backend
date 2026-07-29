using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class AsignacionDispositivo
{
    public int Id { get; set; }

    public int DispositivoId { get; set; }

    public int? PersonaId { get; set; }

    public string? LugarEspecifico { get; set; }

    public int EstadoDispositivoId { get; set; }

    public DateTime FechaAsignacion { get; set; }

    public DateTime? FechaRetiro { get; set; }

    public string? Observaciones { get; set; }

    public string? CreadoPor { get; set; }

    public virtual Dispositivo Dispositivo { get; set; } = null!;

    public virtual EstadoDispositivo EstadoDispositivo { get; set; } = null!;

    public virtual Persona? Persona { get; set; }
}

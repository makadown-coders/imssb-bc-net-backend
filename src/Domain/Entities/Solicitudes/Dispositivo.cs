using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Dispositivo
{
    public int Id { get; set; }

    public int? UnidadMedicaId { get; set; }

    public int? TipoDispositivoId { get; set; }

    public string? Ip { get; set; }

    public string? Conexion { get; set; }

    public string? Serial { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string? Observaciones { get; set; }

    public string? AnydeskId { get; set; }

    public string? RustdeskId { get; set; }

    public string? SupremoId { get; set; }

    public DateTime? CreadoEn { get; set; }

    public DateTime? ActualizadoEn { get; set; }

    public virtual ICollection<AsignacionDispositivo> AsignacionDispositivos { get; set; } = new List<AsignacionDispositivo>();

    public virtual TipoDispositivo? TipoDispositivo { get; set; }

    public virtual UnidadMedica? UnidadMedica { get; set; }
}

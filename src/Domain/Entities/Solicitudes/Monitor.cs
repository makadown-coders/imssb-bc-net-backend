using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Monitor
{
    public int Id { get; set; }

    public int? DispositivoId { get; set; }

    public string? Serial { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public bool? EsPrincipal { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

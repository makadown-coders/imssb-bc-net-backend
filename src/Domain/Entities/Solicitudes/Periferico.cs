using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Periferico
{
    public int Id { get; set; }

    public int? DispositivoId { get; set; }

    public string? Serial { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int TipoId { get; set; }
}

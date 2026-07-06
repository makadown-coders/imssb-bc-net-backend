using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class FactoresConversion
{
    public int Id { get; set; }

    public string Clave { get; set; } = null!;

    public string? SasClave { get; set; }

    public string? Descripcion { get; set; }

    public string? Partida { get; set; }

    public string? DescPartida { get; set; }

    public short? EnDispensacion { get; set; }

    public int? CantidadFc { get; set; }

    public string? PresentacionDisp { get; set; }

    public string? PresentacionPres { get; set; }

    public string? Cluesimb { get; set; }
}

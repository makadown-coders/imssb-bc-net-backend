using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class BalanceoDetalladoFinal
{
    public string? ClaveCnis { get; set; }

    public string? JurisdiccionAlmacen { get; set; }

    public string? JurisdiccionDestino { get; set; }

    public string? CluesDestino { get; set; }

    public string? NombreUnidadDestino { get; set; }

    public int? NecesidadOriginal { get; set; }

    public int? CantidadSugerida { get; set; }

    public int? Prioridad { get; set; }
}

using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class PermBalanceoDetalladoFinal
{
    public int Id { get; set; }

    public int EjecucionId { get; set; }

    public DateTime? FechaEjecucion { get; set; }

    public string ClaveCnis { get; set; } = null!;

    public string JurisdiccionAlmacen { get; set; } = null!;

    public string? JurisdiccionDestino { get; set; }

    public string? CluesDestino { get; set; }

    public string? NombreUnidadDestino { get; set; }

    public int? NecesidadOriginal { get; set; }

    public int? CantidadSugerida { get; set; }

    public int? Prioridad { get; set; }

    public virtual LogEjecucionesBalanceo Ejecucion { get; set; } = null!;
}

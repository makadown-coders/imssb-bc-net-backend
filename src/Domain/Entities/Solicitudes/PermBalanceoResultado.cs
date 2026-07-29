using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class PermBalanceoResultado
{
    public int Id { get; set; }

    public int EjecucionId { get; set; }

    public DateTime? FechaEjecucion { get; set; }

    public string ClaveCnis { get; set; } = null!;

    public string? JurisdiccionOrigen { get; set; }

    public string? JurisdiccionDestino { get; set; }

    public int? CantidadTransferir { get; set; }

    public int? ExistenciaOriginal { get; set; }

    public int? NecesidadDestino { get; set; }

    public virtual LogEjecucionesBalanceo Ejecucion { get; set; } = null!;
}

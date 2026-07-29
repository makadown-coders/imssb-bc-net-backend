using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class BalanceoApartadosHistorial
{
    public long Id { get; set; }

    public int EjecucionId { get; set; }

    public DateTime FechaEjecucion { get; set; }

    public string ClaveCnis { get; set; } = null!;

    public string? CluesAlmacen { get; set; }

    public string? NombreAlmacen { get; set; }

    public string Jurisdiccion { get; set; } = null!;

    public int ExistenciaOriginal { get; set; }

    public int CpmJurisdiccion { get; set; }

    public int CantidadApartada { get; set; }

    public int ExistenciaDisponibleBalanceo { get; set; }

    public string? Observaciones { get; set; }
}

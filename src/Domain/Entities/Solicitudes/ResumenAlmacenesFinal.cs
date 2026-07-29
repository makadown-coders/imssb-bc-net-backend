using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class ResumenAlmacenesFinal
{
    public string? ClaveCnis { get; set; }

    public string? JurisdiccionAlmacen { get; set; }

    public string? JurisdiccionDestino { get; set; }

    public int? TotalUnidades { get; set; }

    public int? TotalPiezas { get; set; }

    public string? InstruccionesDetalladas { get; set; }
}

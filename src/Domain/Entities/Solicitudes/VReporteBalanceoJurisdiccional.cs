using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class VReporteBalanceoJurisdiccional
{
    public int? EjecucionId { get; set; }

    public string? ClaveCnis { get; set; }

    public string? Jurisdiccion { get; set; }

    public int? CpmJurisdiccional { get; set; }

    public int? ExistenciaOriginalAlmacen { get; set; }

    public int? CantidadApartada { get; set; }

    public int? ExistenciaBalanceableInicial { get; set; }

    public int? TransferidoAOtros { get; set; }

    public int? RecibidoDeOtros { get; set; }

    public int? ExcedenteFinal { get; set; }

    public int? DeltaVsCpm { get; set; }

    public bool? CubreCpmJurisdiccional { get; set; }
}

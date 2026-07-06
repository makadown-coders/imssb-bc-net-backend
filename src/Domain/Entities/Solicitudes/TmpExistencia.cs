using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class TmpExistencia
{
    public long Id { get; set; }

    public string Fuente { get; set; } = null!;

    public string? AliasSas { get; set; }

    public string? Cluessa { get; set; }

    public string? Cluesimb { get; set; }

    public string ClaveCnis { get; set; } = null!;

    public string? Lote { get; set; }

    public DateOnly? FechaCaducidad { get; set; }

    public decimal Existencia { get; set; }

    public DateTime CargadoEn { get; set; }
}

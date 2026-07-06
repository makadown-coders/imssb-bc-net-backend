using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class OncoClavesBase
{
    public int Id { get; set; }

    public string Cluesimb { get; set; } = null!;

    public string ClaveCnis { get; set; } = null!;
}

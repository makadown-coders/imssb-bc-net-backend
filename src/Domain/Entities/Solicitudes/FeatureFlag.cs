using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class FeatureFlag
{
    public long Id { get; set; }

    public string FlagKey { get; set; } = null!;

    public string Scope { get; set; } = null!;

    public string? ScopeId { get; set; }

    public string ValueJson { get; set; } = null!;

    public string? Description { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
}

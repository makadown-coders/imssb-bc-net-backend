using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class RadarEventoClafe
{
    public int Id { get; set; }

    public int EventoId { get; set; }

    public string ClaveCnis { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal ExistenciaActual { get; set; }

    public decimal ConsumoPromedio { get; set; }

    public decimal? DiasCobertura { get; set; }

    public decimal CitasPendientes { get; set; }

    public decimal Entradas30d { get; set; }

    public decimal Salidas30d { get; set; }

    public decimal Traspasos30d { get; set; }

    public decimal Solicitado30d { get; set; }

    public int MovimientosRecientes { get; set; }

    public string NivelRiesgo { get; set; } = null!;

    public string Flags { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? RecalculatedAt { get; set; }

    public virtual RadarEvento Evento { get; set; } = null!;
}

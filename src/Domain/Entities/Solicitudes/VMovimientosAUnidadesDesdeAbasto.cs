using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class VMovimientosAUnidadesDesdeAbasto
{
    public string? TipoMovimiento { get; set; }

    public string? CluesDestino { get; set; }

    public string? UnidadDestinoTexto { get; set; }

    public string? UnidadOrigenTexto { get; set; }

    public string? ClaveCnis { get; set; }

    public decimal? Cantidad { get; set; }

    public string? Lote { get; set; }

    public decimal? Total { get; set; }

    public string? Programa { get; set; }

    public DateOnly? FechaMovimiento { get; set; }

    public DateOnly? FechaCaducidad { get; set; }
}

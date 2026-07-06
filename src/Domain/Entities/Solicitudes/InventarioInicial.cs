using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class InventarioInicial
{
    public int Id { get; set; }

    public int? UnidadId { get; set; }

    public string? UnidadTexto { get; set; }

    public string? Partida { get; set; }

    public string? ClaveCnis { get; set; }

    public string? Descripcion { get; set; }

    public string? Lote { get; set; }

    public DateOnly? FechaCaducidad { get; set; }

    public string? Tipo { get; set; }

    public decimal? Cantidad { get; set; }

    public decimal? Costo { get; set; }

    public int? Anio { get; set; }

    public DateTime? CreadoEn { get; set; }

    public virtual UnidadMedicaAlias? Unidad { get; set; }
}

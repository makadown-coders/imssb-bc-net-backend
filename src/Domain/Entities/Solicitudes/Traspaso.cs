using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Traspaso
{
    public int Id { get; set; }

    public DateOnly? FechaRecepcion { get; set; }

    public string? Folio { get; set; }

    public int? UnidadOrigenId { get; set; }

    public string? UnidadOrigenTexto { get; set; }

    public string? ClaveCnis { get; set; }

    public string? Descripcion { get; set; }

    public decimal? Cantidad { get; set; }

    public decimal? Total { get; set; }

    public int? UnidadDestinoId { get; set; }

    public string? UnidadDestinoTexto { get; set; }

    public string? Lote { get; set; }

    public DateOnly? FechaCaducidad { get; set; }

    public string? Partida { get; set; }

    public DateTime? CreadoEn { get; set; }

    public virtual UnidadMedicaAlias? UnidadDestino { get; set; }

    public virtual UnidadMedicaAlias? UnidadOrigen { get; set; }
}

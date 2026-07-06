using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Salidum
{
    public int Id { get; set; }

    public int? UnidadOrigenId { get; set; }

    public string? UnidadOrigenTexto { get; set; }

    public int? UnidadDestinoId { get; set; }

    public string? UnidadDestinoTexto { get; set; }

    public string? Folio { get; set; }

    public string? ClaveCnis { get; set; }

    public decimal? Cantidad { get; set; }

    public decimal? Total { get; set; }

    public string? Programa { get; set; }

    public DateOnly? FechaEntregado { get; set; }

    public string? Tipo { get; set; }

    public string? FolioExtra { get; set; }

    public string? Movto { get; set; }

    public string? Descripcion { get; set; }

    public string? ProgramaExtra { get; set; }

    public string? Lote { get; set; }

    public DateOnly? FechaCaducidad { get; set; }

    public DateTime? CreadoEn { get; set; }

    public virtual UnidadMedicaAlias? UnidadDestino { get; set; }

    public virtual UnidadMedicaAlias? UnidadOrigen { get; set; }
}

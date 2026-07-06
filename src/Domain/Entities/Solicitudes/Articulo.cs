using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class Articulo
{
    public int? Partida { get; set; }

    public string? Clave { get; set; }

    public string? Clavea { get; set; }

    public string? Descripcion { get; set; }

    public string? Presentacion { get; set; }

    public int? Grupogasto { get; set; }

    public int? Subgrupogasto { get; set; }

    public int? Articulo1 { get; set; }

    public string? Categoria { get; set; }

    public string? Ubicacion { get; set; }

    public string? Nivelatencion { get; set; }

    public string? Cbf { get; set; }

    public string? Activo { get; set; }

    public string? Codigobarras { get; set; }

    public int Id { get; set; }
}

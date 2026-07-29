using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class VUnidadMedicaDetalle
{
    public int? Id { get; set; }

    public string? Cluessa { get; set; }

    public string? Cluesimb { get; set; }

    public string? NombreMunicipio { get; set; }

    public string? NombreLocalidad { get; set; }

    public string? NombreTipologia { get; set; }

    public bool? EsSegundoNivel { get; set; }

    public string? NombreDeUnidad { get; set; }

    public string? TipoUnidad { get; set; }

    public string? AliasSas { get; set; }

    public string? Direccion { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public string? EstratoUnidad { get; set; }

    public string? NivelAtencion { get; set; }
}

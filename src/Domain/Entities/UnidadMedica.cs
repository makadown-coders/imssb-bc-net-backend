namespace Domain.Entities;

public sealed class UnidadMedica
{
    public int Id { get; set; }
    public string? Cluessa { get; set; }
    public string? Cluesimb { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
    public string? EstratoUnidad { get; set; }
    public string? NivelAtencion { get; set; }
    public int? TipoUnidadId { get; set; }
    public int? LocalidadId { get; set; }
    public bool Activo { get; set; } = true;
    public TipoUnidad? TipoUnidad { get; set; }
    public Localidad? Localidad { get; set; }
    public TipologiaUnidad? TipologiaUnidad { get; set; }
}

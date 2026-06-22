namespace Domain.Entities;

public sealed class Localidad
{
    public int Id { get; set; }
    public string NombreLocalidad { get; set; } = string.Empty;
    public int? MunicipioId { get; set; }
    public Municipio? Municipio { get; set; }
    public ICollection<UnidadMedica> UnidadesMedicas { get; set; } = new List<UnidadMedica>();
}

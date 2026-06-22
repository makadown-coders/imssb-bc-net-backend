namespace Domain.Entities;

public sealed class Municipio
{
    public int Id { get; set; }
    public string NombreMunicipio { get; set; } = string.Empty;
    public ICollection<Localidad> Localidades { get; set; } = new List<Localidad>();
}

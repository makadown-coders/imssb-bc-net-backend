namespace Domain.Entities;

public sealed class TipoUnidad
{
    public int Id { get; set; }
    public string NombreTipo { get; set; } = string.Empty;
    public ICollection<UnidadMedica> UnidadesMedicas { get; set; } = new List<UnidadMedica>();
}

namespace Domain.Entities;

public sealed class Tipologia
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool? EsSegundoNivel { get; set; } = false;
    public ICollection<TipologiaUnidad> TipologiasUnidad { get; set; } = new List<TipologiaUnidad>();
}

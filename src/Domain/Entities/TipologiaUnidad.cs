namespace Domain.Entities;

public sealed class TipologiaUnidad
{
    public int Id { get; set; }
    public int UnidadMedicaId { get; set; }
    public int TipologiaId { get; set; }
    public string? Fuente { get; set; }
    public DateTime? CreadoEn { get; set; } = DateTime.UtcNow;
    public UnidadMedica? UnidadMedica { get; set; }
    public Tipologia? Tipologia { get; set; }
}

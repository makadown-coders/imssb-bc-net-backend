using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

public partial class UnidadMedica
{
    public int Id { get; set; }

    public string? Cluessa { get; set; }

    public string? Cluesimb { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Direccion { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public string? EstratoUnidad { get; set; }

    public string? NivelAtencion { get; set; }

    public int? TipoUnidadId { get; set; }

    public int? LocalidadId { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cpm> Cpms { get; set; } = new List<Cpm>();

    public virtual ICollection<Dispositivo> Dispositivos { get; set; } = new List<Dispositivo>();

    public virtual Localidad? Localidad { get; set; }

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();

    public virtual TipoUnidad? TipoUnidad { get; set; }

    public virtual UnidadMedicaAlias? UnidadMedicaAlias { get; set; }

    public virtual ICollection<UnidadMedicaKit> UnidadMedicaKits { get; set; } = new List<UnidadMedicaKit>();
}

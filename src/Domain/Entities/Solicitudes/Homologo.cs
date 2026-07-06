using System;
using System.Collections.Generic;

namespace Domain.Entities.Solicitudes;

/// <summary>
/// Tabla de productos homólogos/sustitutos con sus factores de conversión
/// </summary>
public partial class Homologo
{
    /// <summary>
    /// Identificador único autoincrementable
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Código del producto original
    /// </summary>
    public string Clave { get; set; } = null!;

    /// <summary>
    /// Código del producto sustituto
    /// </summary>
    public string Sustituto { get; set; } = null!;

    /// <summary>
    /// Factor de conversión entre productos
    /// </summary>
    public decimal Factor { get; set; }
}

using Application.Features.Solicitudes;
using Infrastructure.Persistence.Solicitudes;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Solicitudes;

internal sealed class CnisGruposTerapeuticosService(SolicitudesDbContext dbContext)
    : ICnisGruposTerapeuticosService
{
    // Proyecciones sin entidades persistentes: no incorporan CNIS al modelo ni a su inicialización.
    public async Task<IReadOnlyList<CnisGrupoTerapeuticoDto>> GetGruposAsync(CancellationToken cancellationToken) =>
        await dbContext.Database.SqlQuery<CnisGrupoTerapeuticoDto>($"""
            SELECT DISTINCT g.numero::integer AS "Numero", g.nombre AS "Nombre"
            FROM public.articulos a
            JOIN cnis.insumo i ON regexp_replace(trim(a.clave), '[^0-9]', '', 'g') = i.clave_normalizada
            JOIN cnis.insumo_grupo ig ON ig.insumo_id = i.id
            JOIN cnis.grupo g ON g.id = ig.grupo_id
            ORDER BY "Numero"
            """).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CnisArticuloGrupoDto>> GetAsociacionesAsync(CancellationToken cancellationToken) =>
        await dbContext.Database.SqlQuery<CnisArticuloGrupoDto>($"""
            SELECT DISTINCT a.clave AS "Clave", g.numero::integer AS "Grupo", g.nombre AS "GrupoNombre"
            FROM public.articulos a
            JOIN cnis.insumo i ON regexp_replace(trim(a.clave), '[^0-9]', '', 'g') = i.clave_normalizada
            JOIN cnis.insumo_grupo ig ON ig.insumo_id = i.id
            JOIN cnis.grupo g ON g.id = ig.grupo_id
            ORDER BY "Grupo", "Clave"
            """).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CnisArticuloDto>> GetArticulosAsync(int numero, CancellationToken cancellationToken) =>
        await dbContext.Database.SqlQuery<CnisArticuloDto>($"""
            SELECT DISTINCT a.clave AS "Clave", a.descripcion AS "Descripcion", a.presentacion AS "Presentacion"
            FROM public.articulos a
            JOIN cnis.insumo i ON regexp_replace(trim(a.clave), '[^0-9]', '', 'g') = i.clave_normalizada
            JOIN cnis.insumo_grupo ig ON ig.insumo_id = i.id
            JOIN cnis.grupo g ON g.id = ig.grupo_id
            WHERE g.numero = {numero}
            ORDER BY "Clave", "Descripcion", "Presentacion"
            """).ToListAsync(cancellationToken);
}

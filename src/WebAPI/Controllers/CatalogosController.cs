using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Contracts;

namespace WebAPI.Controllers;

[ApiController]
[Authorize(Policy = "AdminTic")]
[Route("api/catalogos")]
public sealed class CatalogosController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("tipo-unidad")]
    public async Task<ActionResult<IReadOnlyList<TipoUnidadResponse>>> GetTiposUnidad(
        [FromQuery] string? q,
        CancellationToken cancellationToken)
    {
        var query = dbContext.TiposUnidad.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(tipo => EF.Functions.ILike(tipo.NombreTipo, $"%{q.Trim()}%"));
        }

        var response = await query
            .OrderBy(tipo => tipo.NombreTipo)
            .Select(tipo => new TipoUnidadResponse(tipo.Id, tipo.NombreTipo))
            .ToListAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet("tipo-unidad/{id:int}")]
    public async Task<ActionResult<TipoUnidadResponse>> GetTipoUnidad(int id, CancellationToken cancellationToken)
    {
        var response = await dbContext.TiposUnidad
            .AsNoTracking()
            .Where(tipo => tipo.Id == id)
            .Select(tipo => new TipoUnidadResponse(tipo.Id, tipo.NombreTipo))
            .FirstOrDefaultAsync(cancellationToken);

        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost("tipo-unidad")]
    public async Task<ActionResult<TipoUnidadResponse>> CreateTipoUnidad(TipoUnidadRequest request, CancellationToken cancellationToken)
    {
        var entity = new TipoUnidad { NombreTipo = request.NombreTipo.Trim() };
        dbContext.TiposUnidad.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetTipoUnidad), new { id = entity.Id }, new TipoUnidadResponse(entity.Id, entity.NombreTipo));
    }

    [HttpPut("tipo-unidad/{id:int}")]
    public async Task<IActionResult> UpdateTipoUnidad(int id, TipoUnidadRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.TiposUnidad.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.NombreTipo = request.NombreTipo.Trim();
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("tipo-unidad/{id:int}")]
    public async Task<IActionResult> DeleteTipoUnidad(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.TiposUnidad.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        dbContext.TiposUnidad.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("municipios")]
    public async Task<ActionResult<IReadOnlyList<MunicipioResponse>>> GetMunicipios(
        [FromQuery] string? q,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Municipios.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(municipio => EF.Functions.ILike(municipio.NombreMunicipio, $"%{q.Trim()}%"));
        }

        var response = await query
            .OrderBy(municipio => municipio.NombreMunicipio)
            .Select(municipio => new MunicipioResponse(municipio.Id, municipio.NombreMunicipio))
            .ToListAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet("municipios/{id:int}")]
    public async Task<ActionResult<MunicipioResponse>> GetMunicipio(int id, CancellationToken cancellationToken)
    {
        var response = await dbContext.Municipios
            .AsNoTracking()
            .Where(municipio => municipio.Id == id)
            .Select(municipio => new MunicipioResponse(municipio.Id, municipio.NombreMunicipio))
            .FirstOrDefaultAsync(cancellationToken);

        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost("municipios")]
    public async Task<ActionResult<MunicipioResponse>> CreateMunicipio(MunicipioRequest request, CancellationToken cancellationToken)
    {
        var entity = new Municipio { NombreMunicipio = request.NombreMunicipio.Trim() };
        dbContext.Municipios.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetMunicipio), new { id = entity.Id }, new MunicipioResponse(entity.Id, entity.NombreMunicipio));
    }

    [HttpPut("municipios/{id:int}")]
    public async Task<IActionResult> UpdateMunicipio(int id, MunicipioRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Municipios.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.NombreMunicipio = request.NombreMunicipio.Trim();
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("municipios/{id:int}")]
    public async Task<IActionResult> DeleteMunicipio(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Municipios.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        dbContext.Municipios.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("localidades")]
    public async Task<ActionResult<IReadOnlyList<LocalidadResponse>>> GetLocalidades(
        [FromQuery] int? municipioId,
        [FromQuery] string? q,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Localidades.AsNoTracking();
        if (municipioId.HasValue)
        {
            query = query.Where(localidad => localidad.MunicipioId == municipioId);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(localidad => EF.Functions.ILike(localidad.NombreLocalidad, $"%{q.Trim()}%"));
        }

        var response = await query
            .OrderBy(localidad => localidad.NombreLocalidad)
            .Select(localidad => new LocalidadResponse(
                localidad.Id,
                localidad.NombreLocalidad,
                localidad.MunicipioId,
                localidad.Municipio == null ? null : localidad.Municipio.NombreMunicipio))
            .ToListAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet("localidades/{id:int}")]
    public async Task<ActionResult<LocalidadResponse>> GetLocalidad(int id, CancellationToken cancellationToken)
    {
        var response = await dbContext.Localidades
            .AsNoTracking()
            .Where(localidad => localidad.Id == id)
            .Select(localidad => new LocalidadResponse(
                localidad.Id,
                localidad.NombreLocalidad,
                localidad.MunicipioId,
                localidad.Municipio == null ? null : localidad.Municipio.NombreMunicipio))
            .FirstOrDefaultAsync(cancellationToken);

        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost("localidades")]
    public async Task<ActionResult<LocalidadResponse>> CreateLocalidad(LocalidadRequest request, CancellationToken cancellationToken)
    {
        var entity = new Localidad
        {
            NombreLocalidad = request.NombreLocalidad.Trim(),
            MunicipioId = request.MunicipioId
        };

        dbContext.Localidades.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetLocalidad), new { id = entity.Id }, await BuildLocalidadResponse(entity.Id, cancellationToken));
    }

    [HttpPut("localidades/{id:int}")]
    public async Task<IActionResult> UpdateLocalidad(int id, LocalidadRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Localidades.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.NombreLocalidad = request.NombreLocalidad.Trim();
        entity.MunicipioId = request.MunicipioId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("localidades/{id:int}")]
    public async Task<IActionResult> DeleteLocalidad(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Localidades.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        dbContext.Localidades.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("unidades-medicas")]
    public async Task<ActionResult<IReadOnlyList<UnidadMedicaResponse>>> GetUnidadesMedicas(
        [FromQuery] int? tipoUnidadId,
        [FromQuery] int? localidadId,
        [FromQuery] bool? activo,
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var query = dbContext.UnidadesMedicas.AsNoTracking();
        if (tipoUnidadId.HasValue)
        {
            query = query.Where(unidad => unidad.TipoUnidadId == tipoUnidadId);
        }

        if (localidadId.HasValue)
        {
            query = query.Where(unidad => unidad.LocalidadId == localidadId);
        }

        if (activo.HasValue)
        {
            query = query.Where(unidad => unidad.Activo == activo);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = $"%{q.Trim()}%";
            query = query.Where(unidad =>
                EF.Functions.ILike(unidad.Nombre, search)
                || (unidad.Cluessa != null && EF.Functions.ILike(unidad.Cluessa, search))
                || (unidad.Cluesimb != null && EF.Functions.ILike(unidad.Cluesimb, search)));
        }

        var response = await query
            .OrderBy(unidad => unidad.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(unidad => new UnidadMedicaResponse(
                unidad.Id,
                unidad.Cluessa,
                unidad.Cluesimb,
                unidad.Nombre,
                unidad.Direccion,
                unidad.Latitud,
                unidad.Longitud,
                unidad.EstratoUnidad,
                unidad.NivelAtencion,
                unidad.TipoUnidadId,
                unidad.TipoUnidad == null ? null : unidad.TipoUnidad.NombreTipo,
                unidad.LocalidadId,
                unidad.Localidad == null ? null : unidad.Localidad.NombreLocalidad,
                unidad.Localidad == null ? null : unidad.Localidad.MunicipioId,
                unidad.Localidad == null || unidad.Localidad.Municipio == null ? null : unidad.Localidad.Municipio.NombreMunicipio,
                unidad.Activo))
            .ToListAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet("unidades-medicas/{id:int}")]
    public async Task<ActionResult<UnidadMedicaResponse>> GetUnidadMedica(int id, CancellationToken cancellationToken)
    {
        var response = await BuildUnidadMedicaResponse(id, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost("unidades-medicas")]
    public async Task<ActionResult<UnidadMedicaResponse>> CreateUnidadMedica(UnidadMedicaRequest request, CancellationToken cancellationToken)
    {
        var entity = new UnidadMedica();
        ApplyUnidadMedicaRequest(entity, request);

        dbContext.UnidadesMedicas.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetUnidadMedica), new { id = entity.Id }, await BuildUnidadMedicaResponse(entity.Id, cancellationToken));
    }

    [HttpPut("unidades-medicas/{id:int}")]
    public async Task<IActionResult> UpdateUnidadMedica(int id, UnidadMedicaRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.UnidadesMedicas.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        ApplyUnidadMedicaRequest(entity, request);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("unidades-medicas/{id:int}")]
    public async Task<IActionResult> DeleteUnidadMedica(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.UnidadesMedicas.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Activo = false;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("tipologias")]
    public async Task<ActionResult<IReadOnlyList<TipologiaResponse>>> GetTipologias(
        [FromQuery] string? q,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Tipologias.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(tipologia => EF.Functions.ILike(tipologia.Nombre, $"%{q.Trim()}%"));
        }

        var response = await query
            .OrderBy(tipologia => tipologia.Nombre)
            .Select(tipologia => new TipologiaResponse(tipologia.Id, tipologia.Nombre, tipologia.EsSegundoNivel))
            .ToListAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet("tipologias/{id:int}")]
    public async Task<ActionResult<TipologiaResponse>> GetTipologia(int id, CancellationToken cancellationToken)
    {
        var response = await dbContext.Tipologias
            .AsNoTracking()
            .Where(tipologia => tipologia.Id == id)
            .Select(tipologia => new TipologiaResponse(tipologia.Id, tipologia.Nombre, tipologia.EsSegundoNivel))
            .FirstOrDefaultAsync(cancellationToken);

        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost("tipologias")]
    public async Task<ActionResult<TipologiaResponse>> CreateTipologia(TipologiaRequest request, CancellationToken cancellationToken)
    {
        var entity = new Tipologia
        {
            Nombre = request.Nombre.Trim(),
            EsSegundoNivel = request.EsSegundoNivel
        };

        dbContext.Tipologias.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetTipologia), new { id = entity.Id }, new TipologiaResponse(entity.Id, entity.Nombre, entity.EsSegundoNivel));
    }

    [HttpPut("tipologias/{id:int}")]
    public async Task<IActionResult> UpdateTipologia(int id, TipologiaRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Tipologias.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Nombre = request.Nombre.Trim();
        entity.EsSegundoNivel = request.EsSegundoNivel;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("tipologias/{id:int}")]
    public async Task<IActionResult> DeleteTipologia(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Tipologias.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        dbContext.Tipologias.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("tipologias-unidad")]
    public async Task<ActionResult<IReadOnlyList<TipologiaUnidadResponse>>> GetTipologiasUnidad(
        [FromQuery] int? unidadMedicaId,
        [FromQuery] int? tipologiaId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.TipologiasUnidad.AsNoTracking();
        if (unidadMedicaId.HasValue)
        {
            query = query.Where(item => item.UnidadMedicaId == unidadMedicaId);
        }

        if (tipologiaId.HasValue)
        {
            query = query.Where(item => item.TipologiaId == tipologiaId);
        }

        var response = await query
            .OrderBy(item => item.UnidadMedica!.Nombre)
            .Select(item => new TipologiaUnidadResponse(
                item.Id,
                item.UnidadMedicaId,
                item.UnidadMedica == null ? string.Empty : item.UnidadMedica.Nombre,
                item.TipologiaId,
                item.Tipologia == null ? string.Empty : item.Tipologia.Nombre,
                item.Fuente,
                item.CreadoEn))
            .ToListAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet("tipologias-unidad/{id:int}")]
    public async Task<ActionResult<TipologiaUnidadResponse>> GetTipologiaUnidad(int id, CancellationToken cancellationToken)
    {
        var response = await BuildTipologiaUnidadResponse(id, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost("tipologias-unidad")]
    public async Task<ActionResult<TipologiaUnidadResponse>> CreateTipologiaUnidad(TipologiaUnidadRequest request, CancellationToken cancellationToken)
    {
        var entity = new TipologiaUnidad
        {
            UnidadMedicaId = request.UnidadMedicaId,
            TipologiaId = request.TipologiaId,
            Fuente = request.Fuente?.Trim(),
            CreadoEn = DateTime.UtcNow
        };

        dbContext.TipologiasUnidad.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetTipologiaUnidad), new { id = entity.Id }, await BuildTipologiaUnidadResponse(entity.Id, cancellationToken));
    }

    [HttpPut("tipologias-unidad/{id:int}")]
    public async Task<IActionResult> UpdateTipologiaUnidad(int id, TipologiaUnidadRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.TipologiasUnidad.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.UnidadMedicaId = request.UnidadMedicaId;
        entity.TipologiaId = request.TipologiaId;
        entity.Fuente = request.Fuente?.Trim();
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("tipologias-unidad/{id:int}")]
    public async Task<IActionResult> DeleteTipologiaUnidad(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.TipologiasUnidad.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        dbContext.TipologiasUnidad.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private Task<LocalidadResponse?> BuildLocalidadResponse(int id, CancellationToken cancellationToken)
    {
        return dbContext.Localidades
            .AsNoTracking()
            .Where(localidad => localidad.Id == id)
            .Select(localidad => new LocalidadResponse(
                localidad.Id,
                localidad.NombreLocalidad,
                localidad.MunicipioId,
                localidad.Municipio == null ? null : localidad.Municipio.NombreMunicipio))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private Task<UnidadMedicaResponse?> BuildUnidadMedicaResponse(int id, CancellationToken cancellationToken)
    {
        return dbContext.UnidadesMedicas
            .AsNoTracking()
            .Where(unidad => unidad.Id == id)
            .Select(unidad => new UnidadMedicaResponse(
                unidad.Id,
                unidad.Cluessa,
                unidad.Cluesimb,
                unidad.Nombre,
                unidad.Direccion,
                unidad.Latitud,
                unidad.Longitud,
                unidad.EstratoUnidad,
                unidad.NivelAtencion,
                unidad.TipoUnidadId,
                unidad.TipoUnidad == null ? null : unidad.TipoUnidad.NombreTipo,
                unidad.LocalidadId,
                unidad.Localidad == null ? null : unidad.Localidad.NombreLocalidad,
                unidad.Localidad == null ? null : unidad.Localidad.MunicipioId,
                unidad.Localidad == null || unidad.Localidad.Municipio == null ? null : unidad.Localidad.Municipio.NombreMunicipio,
                unidad.Activo))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private Task<TipologiaUnidadResponse?> BuildTipologiaUnidadResponse(int id, CancellationToken cancellationToken)
    {
        return dbContext.TipologiasUnidad
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new TipologiaUnidadResponse(
                item.Id,
                item.UnidadMedicaId,
                item.UnidadMedica == null ? string.Empty : item.UnidadMedica.Nombre,
                item.TipologiaId,
                item.Tipologia == null ? string.Empty : item.Tipologia.Nombre,
                item.Fuente,
                item.CreadoEn))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static void ApplyUnidadMedicaRequest(UnidadMedica entity, UnidadMedicaRequest request)
    {
        entity.Cluessa = TrimToNull(request.Cluessa);
        entity.Cluesimb = TrimToNull(request.Cluesimb);
        entity.Nombre = request.Nombre.Trim();
        entity.Direccion = TrimToNull(request.Direccion);
        entity.Latitud = request.Latitud;
        entity.Longitud = request.Longitud;
        entity.EstratoUnidad = TrimToNull(request.EstratoUnidad);
        entity.NivelAtencion = TrimToNull(request.NivelAtencion);
        entity.TipoUnidadId = request.TipoUnidadId;
        entity.LocalidadId = request.LocalidadId;
        entity.Activo = request.Activo;
    }

    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

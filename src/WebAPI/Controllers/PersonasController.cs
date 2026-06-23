using System.Security.Claims;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Contracts;

namespace WebAPI.Controllers;

[ApiController]
[Authorize(Policy = "AdminTic")]
[Route("api/personas")]
public sealed class PersonasController(AppDbContext dbContext, IPasswordHasher passwordHasher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PersonaResponse>>> GetPersonas(
        [FromQuery] string? q,
        [FromQuery] bool? activo,
        [FromQuery] int? unidadMedicaId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var query = dbContext.Personas.AsNoTracking();
        if (activo.HasValue)
        {
            query = query.Where(persona => persona.Activo == activo);
        }

        if (unidadMedicaId.HasValue)
        {
            query = query.Where(persona => persona.UnidadMedicaId == unidadMedicaId);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = $"%{q.Trim()}%";
            query = query.Where(persona =>
                EF.Functions.ILike(persona.NombreCompleto, search)
                || (persona.Rfc != null && EF.Functions.ILike(persona.Rfc, search))
                || (persona.Curp != null && EF.Functions.ILike(persona.Curp, search))
                || (persona.CorreoPrincipal != null && EF.Functions.ILike(persona.CorreoPrincipal, search))
                || (persona.Username != null && EF.Functions.ILike(persona.Username, search)));
        }

        var response = await query
            .OrderBy(persona => persona.NombreCompleto)
            .ThenBy(persona => persona.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToResponse())
            .ToListAsync(cancellationToken);

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PersonaResponse>> GetPersona(int id, CancellationToken cancellationToken)
    {
        var response = await BuildPersonaResponse(id, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<PersonaResponse>> CreatePersona(PersonaRequest request, CancellationToken cancellationToken)
    {
        if (!await UnidadMedicaExists(request.UnidadMedicaId, cancellationToken))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(request.UnidadMedicaId)] = ["La unidad médica indicada no existe."]
            }));
        }

        var entity = new Persona();
        ApplyRequest(entity, request);
        dbContext.Personas.Add(entity);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            return Conflict(new { message = "Ya existe una persona con el mismo correo, RFC, CURP o username." });
        }

        return CreatedAtAction(nameof(GetPersona), new { id = entity.Id }, await BuildPersonaResponse(entity.Id, cancellationToken));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePersona(int id, PersonaRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Personas.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (!await UnidadMedicaExists(request.UnidadMedicaId, cancellationToken))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(request.UnidadMedicaId)] = ["La unidad médica indicada no existe."]
            }));
        }

        ApplyRequest(entity, request);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            return Conflict(new { message = "Ya existe una persona con el mismo correo, RFC, CURP o username." });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeactivatePersona(int id, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Personas.FindAsync([id], cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.Activo = false;
        entity.FechaBaja ??= DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:int}/usuario")]
    public async Task<IActionResult> AssociateUser(
        int id,
        AsociarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var persona = await dbContext.Personas.FindAsync([id], cancellationToken);
        if (persona is null)
        {
            return NotFound();
        }

        if (!await dbContext.Users.AnyAsync(user => user.Id == request.UserId, cancellationToken))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(request.UserId)] = ["El usuario indicado no existe."]
            }));
        }

        var isAssociatedWithAnotherPersona = await dbContext.Personas
            .AnyAsync(item => item.Id != id && item.UserId == request.UserId, cancellationToken);
        if (isAssociatedWithAnotherPersona)
        {
            return Conflict(new { message = "El usuario ya está asociado a otra persona." });
        }

        persona.UserId = request.UserId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}/usuario")]
    public async Task<IActionResult> DisassociateUser(int id, CancellationToken cancellationToken)
    {
        var persona = await dbContext.Personas.FindAsync([id], cancellationToken);
        if (persona is null)
        {
            return NotFound();
        }

        persona.UserId = null;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/usuario")]
    public async Task<ActionResult<UsuarioProvisionadoResponse>> ProvisionUser(
        int id,
        ProvisionarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var persona = await dbContext.Personas.FindAsync([id], cancellationToken);
        if (persona is null)
        {
            return NotFound();
        }

        if (!persona.Activo)
        {
            return Conflict(new { message = "No se puede crear una cuenta para una persona inactiva." });
        }

        if (persona.UserId.HasValue)
        {
            return Conflict(new { message = "La persona ya tiene una cuenta asociada." });
        }

        var email = TrimToNull(persona.CorreoPrincipal)?.ToLowerInvariant();
        if (email is null)
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                ["correoPrincipal"] = ["La persona debe tener un correo principal para crear su cuenta."]
            }));
        }

        var roleCode = request.RoleCode.Trim().ToUpperInvariant();
        var roleExists = await dbContext.Roles
            .AnyAsync(role => role.Code == roleCode && role.IsActive, cancellationToken);
        if (!roleExists)
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(request.RoleCode)] = ["El rol indicado no existe o está inactivo."]
            }));
        }

        var emailIsInUse = await dbContext.Users
            .AnyAsync(user => user.Email == email, cancellationToken);
        if (emailIsInUse)
        {
            return Conflict(new { message = "Ya existe una cuenta con el correo principal de esta persona. Usa el endpoint de asociación si corresponde." });
        }

        var assignedByUserId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var currentUserId)
            ? (Guid?)currentUserId
            : null;

        var user = new User
        {
            Email = email,
            PasswordHash = passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        dbContext.Users.Add(user);
        dbContext.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleCode = roleCode,
            AssignedAt = DateTime.UtcNow,
            AssignedByUserId = assignedByUserId,
            IsActive = true
        });
        persona.UserId = user.Id;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            return Conflict(new { message = "No se pudo crear la cuenta porque el correo o la asociación ya existe." });
        }

        return CreatedAtAction(
            nameof(GetPersona),
            new { id },
            new UsuarioProvisionadoResponse(id, user.Id, user.Email, roleCode));
    }

    private Task<PersonaResponse?> BuildPersonaResponse(int id, CancellationToken cancellationToken)
    {
        return dbContext.Personas
            .AsNoTracking()
            .Where(persona => persona.Id == id)
            .Select(ToResponse())
            .FirstOrDefaultAsync(cancellationToken);
    }

    private Task<bool> UnidadMedicaExists(int? unidadMedicaId, CancellationToken cancellationToken)
    {
        return !unidadMedicaId.HasValue
            ? Task.FromResult(true)
            : dbContext.UnidadesMedicas.AnyAsync(unidad => unidad.Id == unidadMedicaId, cancellationToken);
    }

    private static System.Linq.Expressions.Expression<Func<Persona, PersonaResponse>> ToResponse()
    {
        return persona => new PersonaResponse(
            persona.Id,
            persona.NombreCompleto,
            persona.Nombres ?? string.Empty,
            persona.Apellidos ?? string.Empty,
            persona.Cargo,
            persona.UnidadMedicaId,
            persona.UnidadMedica == null ? null : persona.UnidadMedica.Nombre,
            persona.Rfc,
            persona.Curp,
            persona.CorreoPrincipal,
            persona.Username,
            persona.Activo,
            persona.FechaBaja,
            persona.UserId,
            persona.User == null ? null : persona.User.Email,
            persona.CreadoEn,
            persona.ActualizadoEn);
    }

    private static void ApplyRequest(Persona entity, PersonaRequest request)
    {
        entity.Nombres = request.Nombres.Trim();
        entity.Apellidos = request.Apellidos.Trim();
        entity.NombreCompleto = $"{entity.Nombres} {entity.Apellidos}";
        entity.Cargo = TrimToNull(request.Cargo);
        entity.UnidadMedicaId = request.UnidadMedicaId;
        entity.Rfc = TrimToNull(request.Rfc)?.ToUpperInvariant();
        entity.Curp = TrimToNull(request.Curp)?.ToUpperInvariant();
        entity.CorreoPrincipal = TrimToNull(request.CorreoPrincipal)?.ToLowerInvariant();
        entity.Username = TrimToNull(request.Username);
        entity.Activo = request.Activo;
        entity.FechaBaja = request.Activo ? null : entity.FechaBaja ?? DateTime.UtcNow;
        entity.ActualizadoEn = DateTime.UtcNow;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        return exception.InnerException is Npgsql.PostgresException { SqlState: "23505" };
    }

    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

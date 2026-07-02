using System.Security.Claims;
using Application.Commands.ResetUserPassword;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using WebAPI.Contracts;

namespace WebAPI.Controllers;

[ApiController]
[Authorize(Policy = "AdminTic")]
[Route("api/users")]
public sealed class UsersController(AppDbContext dbContext, IClock clock, ISender sender) : ControllerBase
{
    // Este rol se administra únicamente mediante despliegue o mantenimiento directo de la base.
    private const string ProtectedRole = "ADMIN_TIC";

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ManagedUserResponse>>> GetUsers(
        [FromQuery] string? q,
        [FromQuery] bool? isActive,
        [FromQuery] int? unidadId,
        [FromQuery] string? roleCode,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);
        var query = dbContext.Users.AsNoTracking();

        if (isActive.HasValue)
        {
            query = query.Where(user => user.IsActive == isActive.Value);
        }

        if (unidadId.HasValue)
        {
            query = query.Where(user => user.Persona != null && user.Persona.UnidadMedicaId == unidadId.Value);
        }

        if (!string.IsNullOrWhiteSpace(roleCode))
        {
            var normalizedRoleCode = roleCode.Trim().ToUpperInvariant();
            query = query.Where(user => user.UserRoles.Any(userRole =>
                userRole.RoleCode == normalizedRoleCode
                && userRole.IsActive
                && userRole.RevokedAt == null
                && userRole.Role != null
                && userRole.Role.IsActive));
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var search = $"%{q.Trim()}%";
            query = query.Where(user =>
                EF.Functions.ILike(user.Email, search)
                || (user.Persona != null && EF.Functions.ILike(user.Persona.NombreCompleto, search))
                || (user.Persona != null && user.Persona.Rfc != null && EF.Functions.ILike(user.Persona.Rfc, search))
                || (user.Persona != null && user.Persona.Curp != null && EF.Functions.ILike(user.Persona.Curp, search)));
        }

        var users = await query
            .OrderBy(user => user.Persona == null ? user.Email : user.Persona.NombreCompleto)
            .ThenBy(user => user.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new ManagedUserResponse(
                user.Id,
                user.Email,
                user.IsActive,
                user.CreatedAt,
                user.Persona == null ? null : user.Persona.Id,
                user.Persona == null ? null : user.Persona.NombreCompleto,
                user.Persona == null ? null : user.Persona.UnidadMedicaId,
                user.Persona == null || user.Persona.UnidadMedica == null ? null : user.Persona.UnidadMedica.Nombre,
                user.UserRoles
                    .Where(userRole => userRole.IsActive && userRole.RevokedAt == null && userRole.Role != null && userRole.Role.IsActive)
                    .OrderBy(userRole => userRole.RoleCode)
                    .Select(userRole => new UserRoleResponse(
                        userRole.RoleCode,
                        userRole.Role!.Descripcion,
                        userRole.AssignedAt))
                    .ToList()))
            .ToListAsync(cancellationToken);

        return Ok(users);
    }

    [HttpGet("{userId:guid}/roles")]
    public async Task<ActionResult<IReadOnlyList<UserRoleResponse>>> GetUserRoles(
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken))
        {
            return NotFound();
        }

        var roles = await dbContext.UserRoles
            .AsNoTracking()
            .Where(userRole =>
                userRole.UserId == userId
                && userRole.IsActive
                && userRole.RevokedAt == null
                && userRole.Role != null
                && userRole.Role.IsActive)
            .OrderBy(userRole => userRole.RoleCode)
            .Select(userRole => new UserRoleResponse(
                userRole.RoleCode,
                userRole.Role!.Descripcion,
                userRole.AssignedAt))
            .ToListAsync(cancellationToken);

        return Ok(roles);
    }

    [HttpPut("{userId:guid}/password")]
    public async Task<IActionResult> ResetPassword(
        Guid userId,
        ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var administratorUserId = GetCurrentUserId();
        if (!administratorUserId.HasValue)
        {
            return Unauthorized();
        }

        await sender.Send(
            new ResetUserPasswordCommand(administratorUserId.Value, userId, request.NewPassword),
            cancellationToken);
        return NoContent();
    }

    [HttpPost("{userId:guid}/roles/{roleCode}")]
    public async Task<IActionResult> AssignRole(
        Guid userId,
        string roleCode,
        CancellationToken cancellationToken)
    {
        var normalizedRoleCode = roleCode.Trim().ToUpperInvariant();
        if (IsProtectedRole(normalizedRoleCode))
        {
            return ProtectedRoleProblem();
        }

        if (!await dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken))
        {
            return NotFound();
        }

        if (!await dbContext.Roles.AnyAsync(
                role => role.Code == normalizedRoleCode && role.IsActive,
                cancellationToken))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(roleCode)] = ["El rol indicado no existe o está inactivo."]
            }));
        }

        var assignment = await dbContext.UserRoles.FindAsync([userId, normalizedRoleCode], cancellationToken);
        if (assignment is { IsActive: true, RevokedAt: null })
        {
            return NoContent();
        }

        var actorUserId = GetCurrentUserId();
        if (assignment is null)
        {
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleCode = normalizedRoleCode,
                AssignedAt = clock.UtcNow,
                AssignedByUserId = actorUserId,
                IsActive = true
            });
        }
        else
        {
            // Reactivamos la misma relación para conservar una sola fila por Usuario y rol.
            assignment.AssignedAt = clock.UtcNow;
            assignment.AssignedByUserId = actorUserId;
            assignment.IsActive = true;
            assignment.RevokedAt = null;
        }

        await RevokeSessionsAsync(userId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{userId:guid}/roles/{roleCode}")]
    public async Task<IActionResult> RevokeRole(
        Guid userId,
        string roleCode,
        CancellationToken cancellationToken)
    {
        var normalizedRoleCode = roleCode.Trim().ToUpperInvariant();
        if (IsProtectedRole(normalizedRoleCode))
        {
            return ProtectedRoleProblem();
        }

        if (!await dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken))
        {
            return NotFound();
        }

        var assignment = await dbContext.UserRoles.FindAsync([userId, normalizedRoleCode], cancellationToken);
        if (assignment is null || !assignment.IsActive || assignment.RevokedAt.HasValue)
        {
            return NoContent();
        }

        assignment.IsActive = false;
        // La revocación lógica conserva el historial en lugar de eliminar la asignación.
        assignment.RevokedAt = clock.UtcNow;
        await RevokeSessionsAsync(userId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task RevokeSessionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Los roles viajan dentro del JWT; impedir la renovación evita perpetuar claims anteriores.
        var refreshTokens = await dbContext.UserRefreshTokens
            .Where(token => token.UserId == userId && !token.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.Revoke(clock.UtcNow);
        }
    }

    private Guid? GetCurrentUserId() =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : null;

    private static bool IsProtectedRole(string roleCode) =>
        string.Equals(roleCode, ProtectedRole, StringComparison.OrdinalIgnoreCase);

    private ObjectResult ProtectedRoleProblem()
    {
        return Problem(
            statusCode: StatusCodes.Status403Forbidden,
            title: "Rol protegido",
            detail: "ADMIN_TIC no puede asignarse ni revocarse desde este módulo.");
    }
}

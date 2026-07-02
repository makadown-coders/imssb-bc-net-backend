using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Contracts;

namespace WebAPI.Controllers;

[ApiController]
[Authorize(Policy = "AdminTic")]
[Route("api/roles")]
public sealed class RolesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleResponse>>> GetRoles(CancellationToken cancellationToken)
    {
        var roles = await dbContext.Roles
            .AsNoTracking()
            // ADMIN_TIC sigue existiendo, pero no se ofrece como opción asignable a la interfaz.
            .Where(role => role.IsActive && role.Code != "ADMIN_TIC")
            .OrderBy(role => role.Descripcion)
            .Select(role => new RoleResponse(role.Code, role.Descripcion))
            .ToListAsync(cancellationToken);

        return Ok(roles);
    }
}

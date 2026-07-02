using System.Security.Claims;
using Application.Commands.ChangeOwnPassword;
using Application.Commands.ResetUserPassword;
using Application.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Contracts;

namespace WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/user")]
public sealed class UserController(ISender sender) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var response = await sender.Send(new GetCurrentUserQuery(userId), cancellationToken);
        return Ok(response);
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangeOwnPassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        await sender.Send(new ChangeOwnPasswordCommand(userId, request.CurrentPassword, request.NewPassword), cancellationToken);
        return NoContent();
    }

    [HttpPut("{userId:guid}/password")]
    [Authorize(Policy = "AdminTic")]
    public async Task<IActionResult> ResetPassword(Guid userId, ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var administratorUserId))
        {
            return Unauthorized();
        }

        await sender.Send(new ResetUserPasswordCommand(administratorUserId, userId, request.NewPassword), cancellationToken);
        return NoContent();
    }

    private bool TryGetCurrentUserId(out Guid userId) =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
}

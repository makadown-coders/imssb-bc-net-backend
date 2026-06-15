using Application.DTOs;
using Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(IUserRepository users) : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            throw new ValidationException("User not found.");
        }

        return new UserDto(user.Id, user.Email, user.CreatedAt);
    }
}

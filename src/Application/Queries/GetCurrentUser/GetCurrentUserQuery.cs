using Application.DTOs;
using MediatR;

namespace Application.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto>;

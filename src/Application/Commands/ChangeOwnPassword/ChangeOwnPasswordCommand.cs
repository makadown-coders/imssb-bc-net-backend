using MediatR;

namespace Application.Commands.ChangeOwnPassword;

public sealed record ChangeOwnPasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : IRequest;

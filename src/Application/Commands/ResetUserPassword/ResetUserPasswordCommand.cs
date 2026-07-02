using MediatR;

namespace Application.Commands.ResetUserPassword;

public sealed record ResetUserPasswordCommand(
    Guid AdministratorUserId,
    Guid TargetUserId,
    string NewPassword) : IRequest;

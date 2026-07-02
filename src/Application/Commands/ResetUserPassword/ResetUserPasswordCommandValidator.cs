using FluentValidation;

namespace Application.Commands.ResetUserPassword;

public sealed class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(command => command.AdministratorUserId).NotEmpty();
        RuleFor(command => command.TargetUserId)
            .NotEmpty()
            .NotEqual(command => command.AdministratorUserId)
            .WithMessage("Use the personal password endpoint to change your own password.");
        RuleFor(command => command.NewPassword).NotEmpty().MinimumLength(12).MaximumLength(128);
    }
}

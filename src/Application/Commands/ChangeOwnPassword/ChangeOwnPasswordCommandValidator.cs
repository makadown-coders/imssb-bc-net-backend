using FluentValidation;

namespace Application.Commands.ChangeOwnPassword;

public sealed class ChangeOwnPasswordCommandValidator : AbstractValidator<ChangeOwnPasswordCommand>
{
    public ChangeOwnPasswordCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.CurrentPassword).NotEmpty();
        RuleFor(command => command.NewPassword)
            .NotEmpty()
            .MinimumLength(12)
            .MaximumLength(128)
            .NotEqual(command => command.CurrentPassword)
            .WithMessage("The new password must be different from the current password.");
    }
}

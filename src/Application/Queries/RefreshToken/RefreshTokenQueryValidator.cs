using FluentValidation;

namespace Application.Queries.RefreshToken;

public sealed class RefreshTokenQueryValidator : AbstractValidator<RefreshTokenQuery>
{
    public RefreshTokenQueryValidator()
    {
        RuleFor(query => query.RefreshToken).NotEmpty();
    }
}

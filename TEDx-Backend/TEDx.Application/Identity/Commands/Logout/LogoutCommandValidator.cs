using FluentValidation;

namespace TEDx.Application.Identity.Commands.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    private const int MaxRefreshTokenLength = 512;

    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .MaximumLength(MaxRefreshTokenLength)
            .WithMessage("The refresh token is too long.");
    }
}

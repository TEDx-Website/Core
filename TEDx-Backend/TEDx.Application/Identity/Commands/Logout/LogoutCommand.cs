using FluentValidation;
using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.Logout;

public sealed record LogoutCommand(string? RefreshToken)
    : IRequest<Result<Unit>>, IRequireAuthentication;

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

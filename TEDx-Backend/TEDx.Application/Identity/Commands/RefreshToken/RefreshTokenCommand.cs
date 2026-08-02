using FluentValidation;
using MediatR;
using TEDx.Application.Identity.Common;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthTokensResponse>>;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotNull()
            .WithMessage("The refresh token is required.");
    }
}

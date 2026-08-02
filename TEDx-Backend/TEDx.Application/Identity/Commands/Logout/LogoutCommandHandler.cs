using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.Logout;

public sealed class LogoutCommandHandler(
    IRefreshTokenService refreshTokens,
    ICurrentUser currentUser,
    ILogger<LogoutCommandHandler> logger)
    : IRequestHandler<LogoutCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } accountId)
            return Result<Unit>.Failure(Errors_Identity.Unauthenticated);

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            logger.LogInformation(
                "Logout for account {AccountId} without a refresh token; server-side session left intact.",
                accountId);

            return Result<Unit>.Success(Unit.Value);
        }

        await refreshTokens.RevokeForAccountAsync(accountId, request.RefreshToken, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}

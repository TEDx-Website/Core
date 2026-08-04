using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using TEDx.Domain.Identity.Enums;

namespace TEDx.Application.Identity.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    IUserAccountService accounts,
    IRefreshTokenService refreshTokens,
    ICurrentUser currentUser,
    ILogger<ChangePasswordCommandHandler> logger)
    : IRequestHandler<ChangePasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } accountId)
            return Result<Unit>.Failure(Errors_Identity.Unauthenticated);

        var user = await accounts.FindByIdAsync(accountId, cancellationToken);

        if (user is null)
        {
            logger.LogWarning(
                "Change-password for {AccountId} could not resolve the account.", accountId);

            return Result<Unit>.Failure(Errors_Identity.Unauthenticated);
        }

        var changed = await accounts.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword,
            cancellationToken);

        if (changed.IsError)
            return Result<Unit>.Failure(changed.Errors);

        var revoked = await refreshTokens.RevokeAllAsync(
            user.Id,
            ReasonRevoked.Logout,
            cancellationToken);

        logger.LogInformation(
            "Password changed for account {UserId}; {RevokedCount} refresh token(s) revoked.",
            user.Id,
            revoked);

        return Result<Unit>.Success(Unit.Value);
    }
}

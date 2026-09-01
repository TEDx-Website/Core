using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using TEDx.Domain.Identity.Enums;

namespace TEDx.Application.Identity.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IUserAccountService accounts,
    IRefreshTokenService refreshTokens,
    ILogger<ResetPasswordCommandHandler> logger)
    : IRequestHandler<ResetPasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var user = await accounts.FindByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            //await accounts.SimulateResetPasswordAsync(
            //    request.Token,
            //    request.NewPassword,
            //    cancellationToken);

            logger.LogInformation(
                "Password reset rejected: no account for the submitted address.");

            return Result<Unit>.Failure(IdentityErrors.ResetTokenInvalid);
        }
        
        var reset = await accounts.ResetPasswordAsync(
            user,
            request.Token,
            request.NewPassword,
            cancellationToken);

        if (reset.IsError)
            return Result<Unit>.Failure(reset.Errors);

        var revoked = await refreshTokens.RevokeAllAsync(
            user.Id,
            RevocationReason.Logout,
            cancellationToken);

        logger.LogInformation(
            "Password reset for account {UserId}; {RevokedCount} refresh token(s) revoked.",
            user.Id,
            revoked);

        return Result<Unit>.Success(Unit.Value);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Identity.Common;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenService refreshTokens,
    IJwtTokenService jwt,
    IAppDbContext db,
    IClock clock,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, Result<AuthTokensResponse>>
{
    public async Task<Result<AuthTokensResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result<AuthTokensResponse>.Failure(Errors_Identity.TokenInvalid);

        var rotated = await refreshTokens.RotateTokenAsync(
            request.RefreshToken,
            presentedFromIp: null,
            cancellationToken);

        if (rotated.IsError)
            return Result<AuthTokensResponse>.Failure(rotated.Errors);

        // Deactivation blocks refresh as well as login (D:Q10). The rotation already
        // consumed the presented token, so a deactivated user cannot keep extending their
        // session; the 401 also hides that the account exists.
        var user = await db.ApplicationUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Id == rotated.Value.AccountId && u.IsActive,
                cancellationToken);

        if (user is null)
        {
            logger.LogInformation(
                "Refresh rejected: account {AccountId} is inactive or missing.",
                rotated.Value.AccountId);

            return Result<AuthTokensResponse>.Failure(Errors_Identity.TokenInvalid);
        }

        var access = jwt.CreateAccessToken(user);

        var issued = new AuthTokensResponse(
            access.Token,
            access.ExpiresInSeconds,
            rotated.Value.RawToken,
            (int)(rotated.Value.ExpiresAtUtc - clock.UtcNow).TotalSeconds,
            new AuthUserResponse(
                user.Id,
                user.Email ?? string.Empty,
                user.Role.ToString(),
                user.FirstName,
                user.LastName));

        logger.LogInformation(
            "Access token refreshed for account {AccountId}.",
            rotated.Value.AccountId);

        return Result<AuthTokensResponse>.Success(issued);
    }
}

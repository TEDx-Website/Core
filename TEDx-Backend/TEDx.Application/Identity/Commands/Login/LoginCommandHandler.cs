using TEDx.Application.Identity.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Identity.Commands.Login;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.Login;

public sealed class LoginCommandHandler(
    IUserAccountService accounts,
    IJwtTokenService jwt,
    IRefreshTokenService refreshTokens,
    IClock clock,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, Result<AuthTokensResponse>>
{
    public async Task<Result<AuthTokensResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var user = await accounts.FindByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            logger.LogInformation("Login failed: no account for the submitted address.");
            return Result<AuthTokensResponse>.Failure(IdentityErrors.InvalidCredentials);
        }

        var passwordCheck = await accounts.CheckPasswordAsync(
            user,
            request.Password,
            cancellationToken);

        if (passwordCheck is not PasswordCheckResult.Succeeded)
        {
            logger.LogInformation(
                "Login failed for account {UserId} ({Outcome}).",
                user.Id,
                passwordCheck);

            return Result<AuthTokensResponse>.Failure(IdentityErrors.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            logger.LogInformation(
                "Login refused for deactivated account {UserId}.", user.Id);
            return Result<AuthTokensResponse>.Failure(IdentityErrors.AccountDeactivated);
        }

        if (!user.EmailConfirmed)
        {
            logger.LogInformation(
                "Login refused for unconfirmed account {UserId}.", user.Id);
            return Result<AuthTokensResponse>.Failure(IdentityErrors.EmailNotConfirmed);
        }

        var access = jwt.CreateAccessToken(user);

        var refresh = await refreshTokens.GenerateAndStoreAsync(
            user.Id,
            createdByIp: null,
            cancellationToken);

        logger.LogInformation("Login succeeded for account {UserId}.", user.Id);

        var tokens = new AuthTokensResponse(
            access.Token,
            access.ExpiresInSeconds,
            refresh.RawToken,
            (int)(refresh.ExpiresAtUtc - clock.UtcNow).TotalSeconds,
            new AuthUserDto(
                user.Id,
                user.Email ?? email,
                user.Role.ToString(),
                user.FirstName,
                user.LastName));

        return Result<AuthTokensResponse>.Success(tokens);
    }
}

using TEDx.Application.Identity.Dtos;

namespace TEDx.Application.Identity.Commands.Login;

/// <summary>
/// The <c>data</c> payload of <c>POST /api/v1/auth/login</c> and
/// <c>POST /api/v1/auth/refresh</c> — both mint the same token pair.
/// </summary>
public sealed record AuthTokensResponse(
    string AccessToken,
    int AccessTokenExpiresIn,
    string RefreshToken,
    int RefreshTokenExpiresIn,
    AuthUserDto User);

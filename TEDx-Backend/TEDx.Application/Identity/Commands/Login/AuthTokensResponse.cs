namespace TEDx.Application.Identity.Common;

public sealed record AuthTokensResponse(
    string AccessToken,
    int AccessTokenExpiresIn,
    string RefreshToken,
    int RefreshTokenExpiresIn,
    AuthUserResponse User);

public sealed record AuthUserResponse(
    Guid Id,
    string Email,
    string GlobalRole,
    string? FirstName,
    string? LastName);

using TEDx.Domain.Identity.Entities;

namespace TEDx.Application.Common.Interfaces;

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(User user);
}

public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc, int ExpiresInSeconds);
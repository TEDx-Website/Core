namespace TEDx.Application.Common.Interfaces;

public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc, int ExpiresInSeconds);

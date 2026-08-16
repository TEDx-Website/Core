namespace TEDx.Application.Common.Interfaces;

public sealed record RefreshTokenIssued(string RawToken, DateTime ExpiresAtUtc);

namespace TEDx.Application.Common.Interfaces;

public sealed record RefreshTokenRotated(Guid AccountId, string RawToken, DateTime ExpiresAtUtc);

using TEDx.Domain.Common;
using TEDx.Domain.Identity.Enums;

namespace TEDx.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshTokenIssued> GenerateAndStoreAsync(
        Guid accountId,
        string? createdByIp,
        CancellationToken cancellationToken = default);

    Task<Result<RefreshTokenRotated>> RotateTokenAsync(
        string presentedRawToken,
        string? presentedFromIp,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> DetectReuseAsync(
        string presentedRawToken,
        string? presentedFromIp,
        CancellationToken cancellationToken = default);

    Task<bool> RevokeForAccountAsync(
        Guid accountId,
        string presentedRawToken,
        CancellationToken cancellationToken = default);

    Task<int> RevokeAllAsync(
        Guid accountId,
        ReasonRevoked reason,
        CancellationToken cancellationToken = default);
}

public sealed record RefreshTokenIssued(string RawToken, DateTime ExpiresAtUtc);

public sealed record RefreshTokenRotated(Guid AccountId, string RawToken, DateTime ExpiresAtUtc);

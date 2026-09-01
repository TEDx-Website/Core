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

    Task<bool> RevokeForAccountAsync(
        Guid accountId,
        string presentedRawToken,
        CancellationToken cancellationToken = default);

    Task<int> RevokeAllAsync(
        Guid accountId,
        RevocationReason reason,
        CancellationToken cancellationToken = default);
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TEDx.Application.Common;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Identity.Enums;
using TEDx.Infrastructure.Configuration;
using TEDx.Application.Common.Errors;
namespace TEDx.Infrastructure.Identity
using Errors = TEDx.Application.Common.Errors.Errors_Identity;

namespace TEDx.Infrastructure.Identity;

internal sealed class RefreshTokenService : IRefreshTokenService
{
    internal sealed class RefreshTokenService : IRefreshTokenService
    {
        private const int MaxFamilyWalk = 1000;

        private readonly IAppDbContext _db;
        private readonly IClock _clock;
        private readonly JwtOptions _options;
        private readonly ILogger<RefreshTokenService> _logger;

        public RefreshTokenService(
            IAppDbContext db,
            IClock clock,
            IOptions<JwtOptions> options,
            ILogger<RefreshTokenService> logger)
        {
            _db = db;
            _clock = clock;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<RefreshTokenIssued> GenerateAndStoreAsync(
            Guid accountId,
            string? createdByIp,
            CancellationToken cancellationToken = default)
        {
            var issued = AddToken(accountId, createdByIp);
            await _db.SaveChangesAsync(cancellationToken);

            return issued;
        }

        public async Task<Result<RefreshTokenRotated>> RotateTokenAsync(
            string presentedRawToken,
            string? presentedFromIp,
            CancellationToken cancellationToken = default)
        {
            var presented = await FindByRawAsync(presentedRawToken, cancellationToken);

            if (presented is null)
                return Result<RefreshTokenRotated>.Failure(Errors_Identity.TrackForbidden);

            // Revoked is checked BEFORE expiry: a stolen token that has since lapsed is still a
            // break-in signal, and answering TOKEN_INVALID would bury it as a routine timeout.
            if (presented.RevokedAtUtc is not null)
            {
                await RevokeFamilyAsync(presented, presentedFromIp, cancellationToken);
                return Result<RefreshTokenRotated>.Failure(Errors_Identity.TokenReused);
            }

            var now = _clock.UtcNow;

            if (presented.ExpiresAtUtc <= now)
            {
                presented.RevokedAtUtc = now;
                presented.ReasonRevoked = ReasonRevoked.Expired;
                await _db.SaveChangesAsync(cancellationToken);

                return Result<RefreshTokenRotated>.Failure(Errors_Identity.TokenInvalid);
            }

            var replacement = AddToken(presented.AccountId, presentedFromIp);

            presented.RevokedAtUtc = now;
            presented.ReasonRevoked = ReasonRevoked.Rotated;
            presented.ReplacedByTokenHash = RefreshTokenGenerator.Hash(replacement.RawToken);

            await _db.SaveChangesAsync(cancellationToken);

            return Result<RefreshTokenRotated>.Success(
                new RefreshTokenRotated(presented.AccountId, replacement.RawToken, replacement.ExpiresAtUtc));
        }

        public async Task<Result<Guid>> DetectReuseAsync(
            string presentedRawToken,
            string? presentedFromIp,
            CancellationToken cancellationToken = default)
        {
            var presented = await FindByRawAsync(presentedRawToken, cancellationToken);

            if (presented is null)
                return Result<Guid>.Failure(Errors_Identity.TokenInvalid);

            if (presented.RevokedAtUtc is null)
                return Result<Guid>.Success(presented.AccountId);

            await RevokeFamilyAsync(presented, presentedFromIp, cancellationToken);

            return Result<Guid>.Failure(Errors_Identity.TokenReused);
        }

        public async Task<int> RevokeAllAsync(
            Guid accountId,
            ReasonRevoked reason,
            CancellationToken cancellationToken = default)
        {
            var now = _clock.UtcNow;

            var active = await _db.RefreshTokens
                .IgnoreQueryFilters()
                .Where(t => t.AccountId == accountId && t.RevokedAtUtc == null)
                .ToListAsync(cancellationToken);

            if (active.Count == 0)
                return 0;

            foreach (var token in active)
            {
                token.RevokedAtUtc = now;
                token.ReasonRevoked = reason;
            }

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Revoked {Count} refresh token(s) for account {AccountId}. Reason={Reason}.",
                active.Count,
                accountId,
                reason);

            return active.Count;
        }

        private async Task RevokeFamilyAsync(RefreshToken start, string? presentedFromIp, CancellationToken cancellationToken)
        {
            var now = _clock.UtcNow;
            var revoked = 0;

            RefreshToken? current = start;

            while (current is not null && revoked < MaxFamilyWalk)
            {
                current.RevokedAtUtc ??= now;
                current.ReasonRevoked = ReasonRevoked.Reuse;
                revoked++;

                var nextHash = current.ReplacedByTokenHash;

                if (nextHash is null)
                    break;

                current = await _db.RefreshTokens
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(t => t.TokenHash == nextHash, cancellationToken);
            }

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogWarning(
                "Refresh-token reuse detected for account {AccountId} from {Ip}. Revoked {Count} token(s) in the family.",
                start.AccountId,
                presentedFromIp ?? "unknown",
                revoked);
        }

        private RefreshTokenIssued AddToken(Guid accountId, string? createdByIp)
        {
            var raw = RefreshTokenGenerator.CreateRaw();
            var now = _clock.UtcNow;
            var expiresAtUtc = now.AddDays(_options.RefreshTokenDays);

            _db.RefreshTokens.Add(new RefreshToken
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                TokenHash = RefreshTokenGenerator.Hash(raw),
                ExpiresAtUtc = expiresAtUtc,
                CreatedAtUtc = now,
                CreatedByIp = createdByIp,
            });

            return new RefreshTokenIssued(raw, expiresAtUtc);
        }

        private Task<RefreshToken?> FindByRawAsync(string presentedRawToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(presentedRawToken))
                return Task.FromResult<RefreshToken?>(null);

            var hash = RefreshTokenGenerator.Hash(presentedRawToken);

            return _db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);
        }
    }

}

using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Identity.Enums;
using TEDx.Infrastructure.Configuration;
using TEDx.Infrastructure.Persistence;

namespace TEDx.Infrastructure.BackgroundJobs;

public sealed class OutboxAndHoldExpirySweeper : BackgroundService
{
    private const string LockName = "OutboxAndHoldExpirySweeper";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SweeperOptions _options;
    private readonly ILogger<OutboxAndHoldExpirySweeper> _logger;

    public OutboxAndHoldExpirySweeper(
        IServiceScopeFactory scopeFactory,
        IOptions<SweeperOptions> options,
        ILogger<OutboxAndHoldExpirySweeper> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //_logger.LogInformation(
        //    "OutboxAndHoldExpirySweeper started. Interval={IntervalSeconds}s, LockTimeout={LockTimeoutMs}ms",
        //    _options.IntervalSeconds,
        //    _options.LockTimeoutMs);

        //while (!stoppingToken.IsCancellationRequested)
        //{
        //    try
        //    {
        //        await RunSweepAsync(stoppingToken);
        //    }
        //    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        //    {
        //        // Graceful shutdown — exit the loop.
        //        break;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log and continue — the sweeper must never crash the host.
        //        _logger.LogError(ex, "Sweeper tick failed unexpectedly. Will retry on next tick.");
        //    }

        //    await Task.Delay(TimeSpan.FromSeconds(_options.IntervalSeconds), stoppingToken);
        //}

        //_logger.LogInformation("OutboxAndHoldExpirySweeper stopping.");
    }

    //private async Task RunSweepAsync(CancellationToken ct)
    //{
    //    var sw = Stopwatch.StartNew();
    //    _logger.LogInformation("Sweeper tick started.");

    //    await using var scope = _scopeFactory.CreateAsyncScope();
    //    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //    var clock = scope.ServiceProvider.GetRequiredService<IClock>();

    //    await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

    //    // The app-lock is owned by this transaction, so committing or rolling back
    //    // releases it — no explicit release (and no risk of leaking it) is needed.
    //    if (!await TryAcquireAppLockAsync(dbContext, ct))
    //    {
    //        _logger.LogWarning(
    //            "Failed to acquire app-lock '{LockName}' — another instance is sweeping. Skipping this tick.",
    //            LockName);
    //        return;
    //    }

    //    _logger.LogDebug("App-lock '{LockName}' acquired.", LockName);

    //    var utcNow = clock.UtcNow;

    //    // ── (a) Hold-expiry sweep ────────────────────────────────────────────
    //    // TODO S3: Query orders past HoldExpiresAtUtc still PendingPayment
    //    //          → Order.Expire() + release PromoRedemption → Released
    //    //          Uses IX_Order_HoldExpiry filtered index.
    //    _logger.LogInformation(
    //        "Hold-expiry sweep placeholder — 0 orders would be processed at {UtcNow}.",
    //        utcNow);

    //    // ── (b) Outbox drain ─────────────────────────────────────────────────
    //    // TODO S3: Query due unprocessed OutboxMessages (ProcessedAtUtc IS NULL
    //    //          AND NextAttemptAtUtc <= now). Dispatch each, mark processed
    //    //          or increment Attempts + backoff on failure.
    //    //          Uses IX_Outbox_Pending filtered index.
    //    _logger.LogInformation(
    //        "Outbox drain placeholder — 0 messages would be processed at {UtcNow}.",
    //        utcNow);

    //    // ── (c) Refresh-token expiry + retention ─────────────────────────────
    //    await SweepRefreshTokensAsync(dbContext, utcNow, ct);

    //    await dbContext.SaveChangesAsync(ct);
    //    await transaction.CommitAsync(ct);

    //    sw.Stop();
    //    _logger.LogInformation("Sweeper tick completed in {ElapsedMs}ms.", sw.ElapsedMilliseconds);
    //}

    ///// <summary>
    ///// Closes out lapsed refresh tokens with <c>ReasonRevoked = Expired</c> (SM 8) and drops rows
    ///// past the retention window, which would otherwise grow by one per refresh per user forever.
    ///// </summary>
    //private async Task SweepRefreshTokensAsync(ApplicationDbContext dbContext, DateTime utcNow, CancellationToken ct)
    //{
    //    var cutoff = utcNow.AddDays(-_options.RefreshTokenRetentionDays);

    //    // Purge first: deleting rows that were already loaded and modified below would make
    //    // SaveChanges update zero rows and throw.
    //    var purged = await dbContext.RefreshTokens
    //        .IgnoreQueryFilters()
    //        .Where(t => t.ExpiresAtUtc < cutoff)
    //        .ExecuteDeleteAsync(ct);

    //    if (purged > 0)
    //        _logger.LogInformation("Purged {Count} refresh token(s) older than {Cutoff:O}.", purged, cutoff);

    //    var lapsed = await dbContext.RefreshTokens
    //        .IgnoreQueryFilters()
    //        .Where(t => t.RevokedAtUtc == null && t.ExpiresAtUtc <= utcNow)
    //        .ToListAsync(ct);

    //    foreach (var token in lapsed)
    //    {
    //        token.RevokedAtUtc = utcNow;
    //        token.ReasonRevoked = ReasonRevoked.Expired;
    //    }

    //    if (lapsed.Count > 0)
    //        _logger.LogInformation("Marked {Count} refresh token(s) as Expired.", lapsed.Count);
    //}

    ///// <summary>
    ///// Takes a transaction-scoped exclusive <c>sp_getapplock</c> so only one instance sweeps at a time.
    ///// Invoked through ADO.NET rather than <c>SqlQueryRaw</c> because a procedure call returning a
    ///// status code is not composable SQL, and EF would wrap it in an outer <c>SELECT</c>.
    ///// </summary>
    ///// <returns><c>true</c> when the lock was granted (return code 0 or 1); <c>false</c> otherwise.</returns>
    //private async Task<bool> TryAcquireAppLockAsync(ApplicationDbContext dbContext, CancellationToken ct)
    //{
    //    using var command = dbContext.Database.GetDbConnection().CreateCommand();
    //    command.CommandType = CommandType.StoredProcedure;
    //    command.CommandText = "sp_getapplock";
    //    command.Transaction = dbContext.Database.CurrentTransaction?.GetDbTransaction();

    //    AddParameter(command, "@Resource", LockName);
    //    AddParameter(command, "@LockMode", "Exclusive");
    //    AddParameter(command, "@LockTimeout", _options.LockTimeoutMs);

    //    var returnCode = command.CreateParameter();
    //    returnCode.ParameterName = "@ReturnCode";
    //    returnCode.DbType = DbType.Int32;
    //    returnCode.Direction = ParameterDirection.ReturnValue;
    //    command.Parameters.Add(returnCode);

    //    await command.ExecuteNonQueryAsync(ct);

    //    // 0 = granted, 1 = granted after waiting; negative values are timeout/deadlock/error.
    //    return returnCode.Value is int code && code >= 0;
    //}

    //private static void AddParameter(DbCommand command, string name, object value)
    //{
    //    var parameter = command.CreateParameter();
    //    parameter.ParameterName = name;
    //    parameter.Value = value;
    //    command.Parameters.Add(parameter);
    //}
}

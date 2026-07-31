using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TEDx.Application.Common.Interfaces;
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
        _logger.LogInformation(
            "OutboxAndHoldExpirySweeper started. Interval={IntervalSeconds}s, LockTimeout={LockTimeoutMs}ms",
            _options.IntervalSeconds,
            _options.LockTimeoutMs);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunSweepAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown — exit the loop.
                break;
            }
            catch (Exception ex)
            {
                // Log and continue — the sweeper must never crash the host.
                _logger.LogError(ex, "Sweeper tick failed unexpectedly. Will retry on next tick.");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.IntervalSeconds), stoppingToken);
        }

        _logger.LogInformation("OutboxAndHoldExpirySweeper stopping.");
    }

    private async Task RunSweepAsync(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation("Sweeper tick started.");

        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var clock = scope.ServiceProvider.GetRequiredService<IClock>();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);

        var lockAcquired = false;
        try
        {
            lockAcquired = await TryAcquireAppLockAsync(dbContext, ct);

            if (!lockAcquired)
            {
                _logger.LogWarning(
                    "Failed to acquire app-lock '{LockName}' — another instance is sweeping. Skipping this tick.",
                    LockName);
                return;
            }

            _logger.LogDebug("App-lock '{LockName}' acquired.", LockName);

            var utcNow = clock.UtcNow;

            // ── (a) Hold-expiry sweep ────────────────────────────────────────
            // TODO S3: Query orders past HoldExpiresAtUtc still PendingPayment
            //          → Order.Expire() + release PromoRedemption → Released
            //          Uses IX_Order_HoldExpiry filtered index.
            _logger.LogInformation(
                "Hold-expiry sweep placeholder — 0 orders would be processed at {UtcNow}.",
                utcNow);

            // ── (b) Outbox drain ─────────────────────────────────────────────
            // TODO S3: Query due unprocessed OutboxMessages (ProcessedAtUtc IS NULL
            //          AND NextAttemptAtUtc <= now). Dispatch each, mark processed
            //          or increment Attempts + backoff on failure.
            //          Uses IX_Outbox_Pending filtered index.
            _logger.LogInformation(
                "Outbox drain placeholder — 0 messages would be processed at {UtcNow}.",
                utcNow);

            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        finally
        {
            if (lockAcquired)
            {
                await ReleaseAppLockAsync(dbContext);
            }
        }

        sw.Stop();
        _logger.LogInformation("Sweeper tick completed in {ElapsedMs}ms.", sw.ElapsedMilliseconds);
    }

    private async Task<bool> TryAcquireAppLockAsync(AppDbContext dbContext, CancellationToken ct)
    {
        var result = await dbContext.Database
            .SqlQueryRaw<int>(
                "DECLARE @r int; EXEC @r = sp_getapplock @Resource = {0}, @LockMode = 'Exclusive', @LockTimeout = {1}; SELECT @r AS [Value]",
                LockName, _options.LockTimeoutMs)
            .FirstOrDefaultAsync(ct);

        return result >= 0;
    }

    private async Task ReleaseAppLockAsync(AppDbContext dbContext)
    {
        try
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                "EXEC sp_releaseapplock @Resource = {0}",
                LockName);

            _logger.LogDebug("App-lock '{LockName}' released.", LockName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to release app-lock '{LockName}'. It will be released when the transaction ends.", LockName);
        }
    }
}

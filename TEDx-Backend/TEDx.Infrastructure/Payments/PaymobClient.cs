using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TEDx.Application.Ticketing.Payments;
using TEDx.Infrastructure.Options;

namespace TEDx.Infrastructure.Payments;

public sealed class PaymobClient : IPaymobClient
{
    private readonly PaymobOptions _options;
    private readonly ILogger<PaymobClient> _logger;

    public PaymobClient(IOptions<PaymobOptions> options, ILogger<PaymobClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<PaymobPaymentIntention> CreatePaymentIntentionAsync(
        decimal amountEgp,
        string orderReference,
        CancellationToken cancellationToken = default)
    {
        if (amountEgp <= 0m)
            throw new ArgumentOutOfRangeException(nameof(amountEgp), "Payment intentions require a positive amount; free orders bypass the gateway (D:Q18).");
        if (string.IsNullOrWhiteSpace(orderReference))
            throw new ArgumentException("Order reference is required.", nameof(orderReference));

        // Currency crosses the gateway boundary as integer piastres — and only here (D:Q18).
        var amountPiastres = ToPiastres(amountEgp);

        _logger.LogInformation(
            "Paymob intention requested for order {OrderReference} ({AmountPiastres} piastres).",
            orderReference, amountPiastres);

        throw new NotImplementedException("Paymob intention creation is implemented in S3 (payment flow).");
    }

    public bool VerifyHmacSignature(string payload, string signature)
    {
        if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(signature))
            return false;

        var keyBytes = Encoding.UTF8.GetBytes(_options.HmacSecret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA512(keyBytes);
        var computed = hmac.ComputeHash(payloadBytes);
        var computedHex = Convert.ToHexString(computed);

        // Constant-time comparison (case-insensitive hex) to avoid timing side-channels.
        var providedHex = signature.Trim();
        if (computedHex.Length != providedHex.Length)
            return false;

        return CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(computedHex),
            Encoding.ASCII.GetBytes(providedHex.ToUpperInvariant()));
    }

    public PaymobTransactionResult ExtractTransactionResult(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException("Payload is required.", nameof(payload));

        throw new NotImplementedException("Paymob payload extraction is implemented in S3 (payment flow).");
    }

    internal static long ToPiastres(decimal amountEgp) =>
        (long)decimal.Round(amountEgp * 100m, 0, MidpointRounding.AwayFromZero);
}

namespace TEDx.Application.Ticketing.Payments;

public interface IPaymobClient
{
    Task<PaymobPaymentIntention> CreatePaymentIntentionAsync(
        decimal amountEgp,
        string orderReference,
        CancellationToken cancellationToken = default);

    bool VerifyHmacSignature(string payload, string signature);

    PaymobTransactionResult ExtractTransactionResult(string payload);
}

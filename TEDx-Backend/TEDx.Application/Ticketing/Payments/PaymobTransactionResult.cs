namespace TEDx.Application.Ticketing.Payments;

public sealed record PaymobTransactionResult(
    string TransactionId,
    string OrderReference,
    bool IsSuccess,
    long AmountPiastres);

namespace TEDx.Application.Ticketing.Payments;

public sealed record PaymobPaymentIntention(string CheckoutUrl, string IntentionReference);

namespace TEDx.Application.Ticketing.Payments;

public sealed record PaymentIntention(string CheckoutUrl, string IntentionReference);

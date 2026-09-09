namespace TEDx.Infrastructure.Email;

/// <summary>
/// A rendered outbound email: the subject plus both body representations that the
/// SMTP sender attaches as alternate views.
/// </summary>
internal sealed record EmailBody(string Subject, string Html, string PlainText);

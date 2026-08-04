using System.Net;

namespace TEDx.Infrastructure.Email;

internal static class EmailTemplates
{
    internal sealed record EmailBody(string Subject, string Html, string PlainText);

    public static EmailBody EmailConfirmation(string link, int expiryHours) =>
        Build(
            subject: "Confirm your TEDx account",
            heading: "Confirm your email",
            intro: "Thanks for registering. Confirm your email address to activate your account.",
            buttonText: "Confirm email",
            link: link,
            expiryNote: ExpiryNote(expiryHours),
            ignoreNote: "If you did not create this account, you can safely ignore this email.");

    public static EmailBody PasswordReset(string link, int expiryHours) =>
        Build(
            subject: "Reset your TEDx password",
            heading: "Reset your password",
            intro: "We received a request to reset the password for this account.",
            buttonText: "Reset password",
            link: link,
            expiryNote: ExpiryNote(expiryHours),
            ignoreNote: "If you did not request this, you can safely ignore this email — your password will not change.");

    private static string ExpiryNote(int hours) =>
        hours == 1
            ? "This link expires in 1 hour."
            : $"This link expires in {hours} hours.";

    private static EmailBody Build(
        string subject,
        string heading,
        string intro,
        string buttonText,
        string link,
        string expiryNote,
        string ignoreNote)
    {
        // The link is attacker-influenced only via the email address in the query string,
        // so encode it before it reaches the markup.
        var encodedLink = WebUtility.HtmlEncode(link);

        var html =
            $"""
             <!DOCTYPE html>
             <html>
               <body style="font-family: Arial, Helvetica, sans-serif; line-height: 1.5; color: #111;">
                 <h2>{WebUtility.HtmlEncode(heading)}</h2>
                 <p>{WebUtility.HtmlEncode(intro)}</p>
                 <p><a href="{encodedLink}">{WebUtility.HtmlEncode(buttonText)}</a></p>
                 <p>If the link does not work, copy this address into your browser:<br />
                    <span>{encodedLink}</span></p>
                 <p>{WebUtility.HtmlEncode(expiryNote)}</p>
                 <p style="color: #666; font-size: 12px;">{WebUtility.HtmlEncode(ignoreNote)}</p>
               </body>
             </html>
             """;

        var text =
            $"""
             {heading}

             {intro}

             {buttonText}: {link}

             {expiryNote}

             {ignoreNote}
             """;

        return new EmailBody(subject, html, text);
    }
}

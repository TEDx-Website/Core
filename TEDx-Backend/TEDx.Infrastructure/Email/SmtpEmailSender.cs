using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using TEDx.Application.Common.Exceptions;
using TEDx.Application.Common.Interfaces;
using TEDx.Infrastructure.Configuration;

namespace TEDx.Infrastructure.Email;

internal sealed class SmtpEmailSender : IEmailSender
{
    private const int ImplicitTlsPort = 465;

    private readonly SmtpOptions _options;
    private readonly IdentityPolicyOptions _policy;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(
        IOptions<SmtpOptions> options,
        IOptions<IdentityPolicyOptions> policy,
        ILogger<SmtpEmailSender> logger)
    {
        _options = options.Value;
        _policy = policy.Value;
        _logger = logger;
    }

    public Task SendPasswordResetEmailAsync(
        string to,
        string resetLink,
        CancellationToken cancellationToken = default)
    {
        var body = EmailTemplates.PasswordReset(resetLink, _policy.ResetTokenHours);
        return SendAsync(to, body, cancellationToken);
    }

    public Task SendEmailConfirmationEmailAsync(
        string to,
        string confirmLink,
        CancellationToken cancellationToken = default)
    {
        var body = EmailTemplates.EmailConfirmation(confirmLink, _policy.ConfirmTokenHours);
        return SendAsync(to, body, cancellationToken);
    }

    private async Task SendAsync(
        string to,
        EmailTemplates.EmailBody body,
        CancellationToken cancellationToken)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_options.FromAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = body.Subject;
            message.Body = new BodyBuilder
            {
                HtmlBody = body.Html,
                TextBody = body.PlainText,
            }.ToMessageBody();

            using var client = new SmtpClient();

            // SMTP supports two secure connection modes:
            // - SslOnConnect (Implicit TLS): the connection is encrypted from the first byte (typically port 465).
            // - StartTls (Explicit TLS): start with a plain SMTP handshake, then explicitly upgrade the connection
            //   to TLS before authentication or sending mail (typically port 587). We require StartTls rather
            //   than StartTlsWhenAvailable to avoid silently falling back to an unencrypted connection.
            var socketOptions = _options.Port == ImplicitTlsPort
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

            await client.ConnectAsync(_options.Host, _options.Port, socketOptions, cancellationToken);

            if (!string.IsNullOrWhiteSpace(_options.Username))
            {
                await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(quit: true, cancellationToken);

            _logger.LogInformation(
                "Sent {Subject} to {Recipient} via {Host}:{Port}.",
                body.Subject,
                to,
                _options.Host,
                _options.Port);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(
                ex,
                "Failed to send {Subject} to {Recipient} via {Host}:{Port}.",
                body.Subject,
                to,
                _options.Host,
                _options.Port);

            throw new EmailDeliveryException($"Could not send \"{body.Subject}\".", ex);
        }
    }
}

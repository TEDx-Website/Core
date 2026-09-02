using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Exceptions;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.ResendConfirmation;

public sealed class ResendConfirmationCommandHandler(
    IUserAccountService accounts,
    IAuthLinkBuilder linkBuilder,
    IEmailSender emailSender,
    ILogger<ResendConfirmationCommandHandler> logger)
    : IRequestHandler<ResendConfirmationCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(
        ResendConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var user = await accounts.FindByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            logger.LogInformation(
                "Confirmation resend requested for an address with no account; responding neutrally.");

            return Result<Unit>.Success(Unit.Value);
        }

        if (!user.IsActive)
        {
            logger.LogInformation(
                "Confirmation resend requested for deactivated account {UserId}; no email sent.",
                user.Id);

            return Result<Unit>.Success(Unit.Value);
        }

        if (user.EmailConfirmed)
        {
            logger.LogInformation(
                "Confirmation resend requested for already-confirmed account {UserId}; no email sent.",
                user.Id);

            return Result<Unit>.Success(Unit.Value);
        }

        var token = await accounts.GenerateEmailConfirmationTokenAsync(user, cancellationToken);

        var confirmLink = linkBuilder.BuildEmailConfirmation(user.Id, token);

        try
        {
            await emailSender.SendEmailConfirmationEmailAsync(
                user.Email ?? email,
                confirmLink,
                cancellationToken);

            logger.LogInformation(
                "Confirmation email resent for account {UserId}.", user.Id);
        }
        catch (EmailDeliveryException ex)
        {
            logger.LogError(
                ex,
                "Could not resend the confirmation email for account {UserId}.",
                user.Id);
        }

        return Result<Unit>.Success(Unit.Value);
    }
}

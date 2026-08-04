using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Exceptions;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IUserAccountService accounts,
    IAuthLinkBuilder linkBuilder,
    IEmailSender emailSender,
    ILogger<ForgotPasswordCommandHandler> logger)
    : IRequestHandler<ForgotPasswordCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var user = await accounts.FindByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            logger.LogInformation(
                "Password reset requested for an address with no account; responding neutrally.");

            return Result<Unit>.Success(Unit.Value);
        }

        if (!user.IsActive)
        {
            logger.LogInformation(
                "Password reset requested for deactivated account {UserId}; no email sent.",
                user.Id);

            return Result<Unit>.Success(Unit.Value);
        }

        var token = await accounts.GeneratePasswordResetTokenAsync(user, cancellationToken);

        var resetLink = linkBuilder.BuildPasswordReset(user.Email ?? email, token);

        try
        {
            await emailSender.SendPasswordResetEmailAsync(
                user.Email ?? email,
                resetLink,
                cancellationToken);

            logger.LogInformation("Password reset email dispatched for account {UserId}.", user.Id);
        }
        catch (EmailDeliveryException ex)
        {
            logger.LogError(
                ex,
                "Could not send the password reset email for account {UserId}.",
                user.Id);
        }

        return Result<Unit>.Success(Unit.Value);
    }
}

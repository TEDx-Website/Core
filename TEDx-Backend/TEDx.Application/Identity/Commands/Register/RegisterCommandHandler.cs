using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Exceptions;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using TEDx.Domain.Identity.Entities;

namespace TEDx.Application.Identity.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserAccountService accounts,
    IAuthLinkBuilder linkBuilder,
    IEmailSender emailSender,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var created = await accounts.CreateAttendeeAsync(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            email,
            request.Password,
            cancellationToken);

        if (created.IsError)
            return Result<RegisterResponse>.Failure(created.Errors);

        var user = created.Value;

        logger.LogInformation(
            "Registered account {UserId} with the Attendee role, pending email confirmation.",
            user.Id);

        await SendConfirmationEmailAsync(user, user.Email ?? email, cancellationToken);

        var response = new RegisterResponse(
            user.Id,
            user.Email ?? email,
            user.FirstName ?? string.Empty,
            user.LastName ?? string.Empty,
            user.Role.ToString(),
            EmailConfirmationRequired: true);

        return Result<RegisterResponse>.Success(response);
    }

    private async Task SendConfirmationEmailAsync(
        User user,
        string email,
        CancellationToken cancellationToken)
    {
        var token = await accounts.GenerateEmailConfirmationTokenAsync(user, cancellationToken);

        var confirmLink = linkBuilder.BuildEmailConfirmation(email, token);

        try
        {
            await emailSender.SendEmailConfirmationEmailAsync(email, confirmLink, cancellationToken);

            logger.LogInformation(
                "Confirmation email dispatched for account {UserId}.", user.Id);
        }
        catch (EmailDeliveryException ex)
        {
            logger.LogError(
                ex,
                "Registered account {UserId} but could not send the confirmation email.",
                user.Id);
        }
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(
    IUserAccountService accounts,
    ILogger<ConfirmEmailCommandHandler> logger)
    : IRequestHandler<ConfirmEmailCommand, Result<ConfirmEmailResponse>>
{
    public async Task<Result<ConfirmEmailResponse>> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        var user = await accounts.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            logger.LogInformation(
                "Email confirmation rejected: no account for the submitted address.");

            return Result<ConfirmEmailResponse>.Failure(IdentityErrors.ConfirmTokenInvalid);
        }

        // Confirming twice is a link opened twice, not an error worth surfacing.
        if (user.EmailConfirmed)
        {
            logger.LogInformation(
                "Email confirmation for account {UserId} was already complete.", user.Id);

            return Result<ConfirmEmailResponse>.Success(new ConfirmEmailResponse(user.Email!, true));
        }

        var confirmed = await accounts.ConfirmEmailAsync(user, request.Token, cancellationToken);

        if (confirmed.IsError)
            return Result<ConfirmEmailResponse>.Failure(confirmed.Errors);

        logger.LogInformation("Email confirmed for account {UserId}.", user.Id);

        return Result<ConfirmEmailResponse>.Success(new ConfirmEmailResponse(user.Email!, true));
    }
}

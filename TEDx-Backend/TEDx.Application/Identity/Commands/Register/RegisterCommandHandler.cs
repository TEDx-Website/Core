using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserAccountService accounts,
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

        var response = new RegisterResponse(
            user.Id,
            user.Email ?? email,
            user.FirstName ?? string.Empty,
            user.LastName ?? string.Empty,
            user.Role.ToString(),
            EmailConfirmationRequired: true);

        return Result<RegisterResponse>.Success(response);
    }
}

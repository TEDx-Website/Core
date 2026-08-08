using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Identity.Enums;
using Errors_Common = TEDx.Application.Common.Errors.Errors_Common;
using Errors = TEDx.Application.Common.Errors.Errors_Identity;

namespace TEDx.Infrastructure.Identity;

internal sealed class UserAccountService : IUserAccountService
{
    private const string DuplicateEmailCode = "DuplicateEmail";
    private const string DuplicateUserNameCode = "DuplicateUserName";
    private const string InvalidTokenCode = "InvalidToken";
    private const string PasswordMismatchCode = "PasswordMismatch";

    private readonly UserManager<User> _userManager;
    private readonly IClock _clock;
    private readonly ILogger<UserAccountService> _logger;

    public UserAccountService(
        UserManager<User> userManager,
        IClock clock,
        ILogger<UserAccountService> logger)
    {
        _userManager = userManager;
        _clock = clock;
        _logger = logger;
    }

    public Task<User?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _userManager.FindByEmailAsync(email);
    }

    public async Task<Result<User>> CreateAttendeeAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = false,
            Role = GlobalRole.Attendee,
            IsActive = true,
            CreatedAtUtc = _clock.UtcNow,
        };

        var created = await _userManager.CreateAsync(user, password);
        if (created.Succeeded)
            return Result<User>.Success(user);

        if (created.Errors.Any(e =>
                e.Code is DuplicateEmailCode or DuplicateUserNameCode))
        {
            return Result<User>.Failure(Errors.EmailTaken);
        }

        var validationErrors = created.Errors
            .Select(e => Error.Validation(
                Errors_Common.ValidationError.Code,
                e.Description,
                field: "password"))
            .ToList();

        _logger.LogWarning(
            "Identity rejected a registration that passed validation: {Codes}",
            string.Join(",", created.Errors.Select(e => e.Code)));

        return Result<User>.Failure(validationErrors);
    }

    public async Task<PasswordCheckResult> CheckPasswordAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (await _userManager.IsLockedOutAsync(user))
        {
            _logger.LogInformation(
                "Login refused for locked-out account {UserId}.", user.Id);
            return PasswordCheckResult.LockedOut;
        }

        if (await _userManager.CheckPasswordAsync(user, password))
        {
            await _userManager.ResetAccessFailedCountAsync(user);
            return PasswordCheckResult.Succeeded;
        }

        var accessFailed = await _userManager.AccessFailedAsync(user);
        if (!accessFailed.Succeeded)
        {
            _logger.LogWarning(
                "Failed to record a failed access attempt for {UserId}.", user.Id);
        }

        // The increment may have just crossed the threshold; report the lockout so it is
        // logged distinctly, even though the client sees the same generic 401.
        return await _userManager.IsLockedOutAsync(user)
            ? PasswordCheckResult.LockedOut
            : PasswordCheckResult.Failed;
    }

    public Task<User?> FindByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return _userManager.FindByIdAsync(userId.ToString());
    }

    public Task<string> GeneratePasswordResetTokenAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        return _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<Result<Unit>> ResetPasswordAsync(
        User user,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded)
            return Result<Unit>.Success(Unit.Value);

        if (result.Errors.Any(e => e.Code == InvalidTokenCode))
        {
            _logger.LogInformation(
                "Rejected a password reset for {UserId}: the token was invalid, expired, or already used.",
                user.Id);

            return Result<Unit>.Failure(Errors.ResetTokenInvalid);
        }

        return Result<Unit>.Failure(ToPasswordValidationErrors(result, user.Id));
    }

    public async Task<Result<Unit>> ChangePasswordAsync(
        User user,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
            return Result<Unit>.Success(Unit.Value);

        if (result.Errors.Any(e => e.Code == PasswordMismatchCode))
        {
            _logger.LogInformation(
                "Rejected a password change for {UserId}: the current password did not match.",
                user.Id);

            return Result<Unit>.Failure(Errors.CurrentPasswordIncorrect);
        }

        return Result<Unit>.Failure(ToPasswordValidationErrors(result, user.Id));
    }

    public Task<string> GenerateEmailConfirmationTokenAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        return _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<Result<Unit>> ConfirmEmailAsync(
        User user,
        string token,
        CancellationToken cancellationToken = default)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
            return Result<Unit>.Success(Unit.Value);

        if (result.Errors.Any(e => e.Code == InvalidTokenCode))
        {
            _logger.LogInformation(
                "Rejected an email confirmation for {UserId}: the token was invalid, expired, or already used.",
                user.Id);
        }
        else
        {
            _logger.LogWarning(
                "Identity rejected an email confirmation for {UserId}: {Codes}",
                user.Id,
                string.Join(",", result.Errors.Select(e => e.Code)));
        }

        return Result<Unit>.Failure(Errors.ConfirmTokenInvalid);
    }

    public async Task<Result<Unit>> UpdateProfilePictureAsync(
        User user,
        string profilePictureUrl,
        CancellationToken cancellationToken = default)
    {
        user.ProfilePictureUrl = profilePictureUrl;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            _logger.LogInformation(
                "Profile picture updated for account {UserId}.", user.Id);

            return Result<Unit>.Success(Unit.Value);
        }

        _logger.LogError(
            "Identity rejected a profile-picture update for {UserId}: {Codes}",
            user.Id,
            string.Join(",", result.Errors.Select(e => e.Code)));

        return Result<Unit>.Failure(Errors.ProfileUpdateFailed);
    }

    private IReadOnlyList<Error> ToPasswordValidationErrors(IdentityResult result, Guid userId)
    {
        _logger.LogWarning(
            "Identity rejected a password that passed validation for {UserId}: {Codes}",
            userId,
            string.Join(",", result.Errors.Select(e => e.Code)));

        return result.Errors
            .Select(e => Error.Validation(
                Errors_Common.ValidationError.Code,
                e.Description,
                field: "newPassword"))
            .ToList();
    }
}

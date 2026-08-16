using MediatR;
using TEDx.Application.Common.Errors;
using TEDx.Domain.Common;
using TEDx.Domain.Identity.Entities;

namespace TEDx.Application.Common.Interfaces;

public interface IUserAccountService
{
    Task<User?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result<User>> CreateAttendeeAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<PasswordCheckResult> CheckPasswordAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default);

    Task<User?> FindByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<string> GeneratePasswordResetTokenAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<Result<Unit>> ResetPasswordAsync(
        User user,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<Result<Unit>> ChangePasswordAsync(
        User user,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<string> GenerateEmailConfirmationTokenAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<Result<Unit>> ConfirmEmailAsync(
        User user,
        string token,
        CancellationToken cancellationToken = default);

    Task<Result<Unit>> UpdateProfilePictureAsync(
        User user,
        string profilePictureUrl,
        CancellationToken cancellationToken = default);
}

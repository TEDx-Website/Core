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
}

public enum PasswordCheckResult
{
    Succeeded = 0,
    Failed = 1,
    LockedOut = 2,
}

namespace TEDx.Application.Identity.Dtos;

/// <summary>
/// The <c>user</c> block nested inside <see cref="Commands.Login.AuthTokensResponse"/>.
/// A piece of a payload, never a payload on its own — hence <c>Dto</c>, not <c>Response</c>.
/// </summary>
public sealed record AuthUserDto(
    Guid Id,
    string Email,
    string GlobalRole,
    string? FirstName,
    string? LastName);

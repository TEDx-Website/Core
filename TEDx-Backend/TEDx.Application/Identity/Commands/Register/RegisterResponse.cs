namespace TEDx.Application.Identity.Commands.Register;

public sealed record RegisterResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string GlobalRole,
    bool EmailConfirmationRequired);

using MediatR;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.Register;

public sealed record RegisterCommand( // why Command? Because it is a request to perform an action, in this case, registering a new user. In the CQRS (Command Query Responsibility Segregation) pattern, commands are used to change the state of the system, while queries are used to retrieve data without modifying the state.
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword)
    : IRequest<Result<RegisterResponse>>;

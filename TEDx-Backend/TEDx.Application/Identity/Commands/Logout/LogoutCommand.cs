using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.Logout;

public sealed record LogoutCommand(string? RefreshToken)
    : IRequest<Result<Unit>>, IRequireAuthentication;

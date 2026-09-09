using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Application.Identity.Queries.GetMyProfile;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.UpdateMyProfile;

public sealed record UpdateMyProfileCommand(
    string? FirstName,
    string? LastName,
    string? Phone,
    string? Bio) : IRequest<Result<MyProfileResponse>>, IRequireAuthentication;

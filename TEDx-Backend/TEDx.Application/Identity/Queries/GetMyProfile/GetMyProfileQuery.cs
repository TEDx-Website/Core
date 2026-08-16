using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Queries.GetMyProfile;

public sealed record GetMyProfileQuery : IRequest<Result<MyProfileResponse>>, IRequireAuthentication;

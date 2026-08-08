using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.UploadProfilePicture;

public sealed record UploadProfilePictureCommand(
    Stream Content,
    string FileName)
    : IRequest<Result<ProfilePictureResponse>>, IRequireAuthentication;

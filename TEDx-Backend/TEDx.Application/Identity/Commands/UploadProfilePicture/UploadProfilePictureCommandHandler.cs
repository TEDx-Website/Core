using MediatR;
using Microsoft.Extensions.Logging;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.UploadProfilePicture;

public sealed class UploadProfilePictureCommandHandler(
    IImageUploadService images,
    IUserAccountService accounts,
    ICurrentUser currentUser,
    ILogger<UploadProfilePictureCommandHandler> logger)
    : IRequestHandler<UploadProfilePictureCommand, Result<ProfilePictureResponse>>
{
    public async Task<Result<ProfilePictureResponse>> Handle(
        UploadProfilePictureCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } accountId)
            return Result<ProfilePictureResponse>.Failure(Errors_Identity.Unauthenticated);

        var user = await accounts.FindByIdAsync(accountId, cancellationToken);

        if (user is null)
        {
            logger.LogWarning(
                "Profile-picture upload for {AccountId} could not resolve the account.", accountId);

            return Result<ProfilePictureResponse>.Failure(Errors_Identity.Unauthenticated);
        }

        // Upload first: a failed upload must leave the existing avatar untouched.
        var uploaded = await images.UploadAsync(
            request.Content,
            request.FileName,
            cancellationToken);

        if (uploaded.IsError)
            return Result<ProfilePictureResponse>.Failure(uploaded.Errors);

        var persisted = await accounts.UpdateProfilePictureAsync(
            user,
            uploaded.Value,
            cancellationToken);

        if (persisted.IsError)
            return Result<ProfilePictureResponse>.Failure(persisted.Errors);

        return Result<ProfilePictureResponse>.Success(
            new ProfilePictureResponse(uploaded.Value));
    }
}

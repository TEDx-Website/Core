using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Identity.Queries.GetMyProfile;
using TEDx.Application.Identity.Services;
using TEDx.Domain.Common;

namespace TEDx.Application.Identity.Commands.UpdateMyProfile;

public sealed class UpdateMyProfileCommandHandler
    : IRequestHandler<UpdateMyProfileCommand, Result<MyProfileResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IMyProfileService _myProfileService;

    public UpdateMyProfileCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        IMyProfileService myProfileService)
    {
        _context = context;
        _currentUser = currentUser;
        _myProfileService = myProfileService;
    }

    public async Task<Result<MyProfileResponse>> Handle(
        UpdateMyProfileCommand request,
        CancellationToken cancellationToken)
    {
        // IRequireAuthentication already rejected the anonymous case; this guard keeps
        // the handler honest if the marker is ever dropped.
        var userId = _currentUser.UserId;
        if (userId is null || userId == Guid.Empty)
        {
            return Result<MyProfileResponse>.Failure(IdentityErrors.UserNotFound);
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            return Result<MyProfileResponse>.Failure(IdentityErrors.UserNotFound);
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.Phone;
        user.Bio = request.Bio;

        await _context.SaveChangesAsync(cancellationToken);

        var profile = await _myProfileService.GetMyProfileAsync(userId.Value, cancellationToken);

        if (profile is null)
        {
            return Result<MyProfileResponse>.Failure(IdentityErrors.UserNotFound);
        }

        return Result<MyProfileResponse>.Success(profile);
    }
}

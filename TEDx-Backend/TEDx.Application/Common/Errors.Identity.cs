using TEDx.Domain.Common;

namespace TEDx.Application.Common;

public static partial class Errors
{
    public static readonly Error Unauthenticated =
        new("UNAUTHENTICATED", "Authentication is required.", ErrorType.Unauthorized);

    public static readonly Error Forbidden =
        new("FORBIDDEN", "You do not have permission to perform this action.", ErrorType.Unauthorized);

    public static readonly Error TrackForbidden =
        new("TRACK_FORBIDDEN", "You do not have permission for this track.", ErrorType.Unauthorized);

    public static readonly Error EmailTaken =
        new("EMAIL_TAKEN", "This email address is already registered.", ErrorType.Conflict);

    public static readonly Error InvalidCredentials =
        new("INVALID_CREDENTIALS", "Invalid email or password.", ErrorType.Unauthorized);

    public static readonly Error CurrentPasswordIncorrect =
        new("CURRENT_PASSWORD_INCORRECT", "The current password is incorrect.", ErrorType.Business);

    public static readonly Error AccountDeactivated =
        new("ACCOUNT_DEACTIVATED", "This account has been deactivated.", ErrorType.Unauthorized);

    public static readonly Error WeakPassword =
        new("WEAK_PASSWORD", "The password does not meet the strength policy.", ErrorType.Validation);

    public static readonly Error TokenInvalid =
        new("TOKEN_INVALID", "The token is expired or unknown.", ErrorType.Unauthorized);

    public static readonly Error TokenReused =
        new("TOKEN_REUSED", "The token has already been used; re-authentication is required.", ErrorType.Unauthorized);

    public static readonly Error ResetTokenInvalid =
        new("RESET_TOKEN_INVALID", "The password-reset token is invalid or expired.", ErrorType.Business);

    public static readonly Error UserNotFound =
        new("USER_NOT_FOUND", "The specified user was not found.", ErrorType.NotFound);

    // Role/assignment conflicts (API §3).
    public static readonly Error MemberBoardSameTrack =
        new("MEMBER_BOARD_SAME_TRACK", "A user cannot be Member and Board of the same track.", ErrorType.Conflict);

    public static readonly Error DualRoleConflict =
        new("DUAL_ROLE_CONFLICT", "This role assignment conflicts with an existing one.", ErrorType.Conflict);

    public static readonly Error TrackAlreadyHasBoard =
        new("TRACK_ALREADY_HAS_BOARD", "This track already has a Board member.", ErrorType.Conflict);

    public static readonly Error AlreadyMemberElsewhere =
        new("ALREADY_MEMBER_ELSEWHERE", "This user is already a Member of another track.", ErrorType.Conflict);
}

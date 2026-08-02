using TEDx.Domain.Common;

namespace TEDx.Application.Common.Errors;

public static partial class Errors_Identity
{
    public static readonly Error Unauthenticated =
        Error.Unauthorized("UNAUTHENTICATED", "Authentication is required.");
   
    public static readonly Error Forbidden =
        Error.Forbidden(
            "FORBIDDEN",
            "You do not have permission to perform this action.");

    public static readonly Error TrackForbidden =
        Error.Forbidden(
            "TRACK_FORBIDDEN",
            "You do not have permission for this track.");

    public static readonly Error EmailTaken =
        Error.Conflict(
            "EMAIL_TAKEN",
            "This email address is already registered.");

    public static readonly Error InvalidCredentials =
        Error.Unauthorized(
            "INVALID_CREDENTIALS",
            "Invalid email or password.");

    public static readonly Error CurrentPasswordIncorrect =
        Error.Business(
            "CURRENT_PASSWORD_INCORRECT",
            "The current password is incorrect.");

    public static readonly Error AccountDeactivated =
        Error.Unauthorized(
            "ACCOUNT_DEACTIVATED",
            "This account has been deactivated.");

    public static readonly Error WeakPassword =
        Error.Validation(
            "WEAK_PASSWORD",
            "The password does not meet the strength policy.");

    public static readonly Error TokenInvalid =
        Error.Unauthorized(
            "TOKEN_INVALID",
            "The token is expired or unknown.");

    public static readonly Error TokenReused =
        Error.Unauthorized(
            "TOKEN_REUSED",
            "The token has already been used; re-authentication is required.");

    public static readonly Error ResetTokenInvalid =
        Error.BadRequest(
            "RESET_TOKEN_INVALID",
            "The password-reset token is invalid or expired.");

    public static readonly Error UserNotFound =
        Error.NotFound(
            "USER_NOT_FOUND",
            "The specified user was not found.");

    public static readonly Error MemberBoardSameTrack =
        Error.Conflict(
            "MEMBER_BOARD_SAME_TRACK",
            "A user cannot be Member and Board of the same track.");

    public static readonly Error DualRoleConflict =
        Error.Conflict(
            "DUAL_ROLE_CONFLICT",
            "This role assignment conflicts with an existing one.");

    public static readonly Error TrackAlreadyHasBoard =
        Error.Conflict(
            "TRACK_ALREADY_HAS_BOARD",
            "This track already has a Board member.");

    public static readonly Error AlreadyMemberElsewhere =
        Error.Conflict(
            "ALREADY_MEMBER_ELSEWHERE",
            "This user is already a Member of another track.");
}

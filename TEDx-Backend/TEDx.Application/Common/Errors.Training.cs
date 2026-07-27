using TEDx.Domain.Common;

namespace TEDx.Application.Common;

public static partial class Errors
{
    public static readonly Error TrackNameTaken =
    Error.Conflict(
        "TRACK_NAME_TAKEN",
        "A track with this name already exists.");

    public static readonly Error SessionHasRecords =
        Error.Conflict(
            "SESSION_HAS_RECORDS",
            "The session has attendance or evaluation records.");

    public static readonly Error SessionNotOccurred =
        Error.Business(
            "SESSION_NOT_OCCURRED",
            "The session has not occurred yet.");

    public static readonly Error MemberNotEnrolled =
        Error.Business(
            "MEMBER_NOT_ENROLLED",
            "The member is not enrolled in this track.");

    public static readonly Error EnrollmentNotInTrack =
        Error.NotFound(
            "ENROLLMENT_NOT_IN_TRACK",
            "The enrollment does not belong to this track.");

    public static readonly Error InvalidScore =
        Error.Validation(
            "INVALID_SCORE",
            "The evaluation score is out of range.");

}

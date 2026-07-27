using TEDx.Domain.Common;

namespace TEDx.Application.Common;

public static partial class Errors
{
    public static readonly Error TrackNameTaken =
        new("TRACK_NAME_TAKEN", "A track with this name already exists.", ErrorType.Conflict);

    public static readonly Error SessionHasRecords =
        new("SESSION_HAS_RECORDS", "The session has attendance or evaluation records.", ErrorType.Conflict);

    public static readonly Error SessionNotOccurred =
        new("SESSION_NOT_OCCURRED", "The session has not occurred yet.", ErrorType.Business);

    public static readonly Error MemberNotEnrolled =
        new("MEMBER_NOT_ENROLLED", "The member is not enrolled in this track.", ErrorType.Business);

    public static readonly Error EnrollmentNotInTrack =
        new("ENROLLMENT_NOT_IN_TRACK", "The enrollment does not belong to this track.", ErrorType.NotFound);

    public static readonly Error InvalidScore =
        new("INVALID_SCORE", "The evaluation score is out of range.", ErrorType.Validation);
}

namespace TEDx.Application.Identity.Dtos;

public sealed record TrackAssignmentDto(
    Guid? MemberOfTrackId,
    Guid? BoardOfTrackId);

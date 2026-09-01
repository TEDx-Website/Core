using TEDx.Domain.Communication.Enums;

namespace TEDx.Application.Communication.Dtos;

public sealed record ContactSubmissionListItemDto(
    Guid Id,
    string Name,
    string Email,
    string Subject,
    string MessageExcerpt,
    ContactStatus Status,
    DateTime CreatedAtUtc);

using TEDx.Domain.Communication.Enums;
namespace TEDx.Application.Ticketing.Queries.GetContactSubmissionById
{
    public sealed record GetContactSubmissionByIdResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string Subject { get; init; } = default!;
        public string Message { get; init; } = default!;
        public ContactStatus Status { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateTime? UpdatedAtUtc { get; init; }
        public string? UpdatedBy { get; init; }
    }
}


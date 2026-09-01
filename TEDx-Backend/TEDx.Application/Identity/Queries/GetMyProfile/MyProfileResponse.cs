using TEDx.Application.Identity.Dtos;
using TEDx.Domain.Identity.Enums;

namespace TEDx.Application.Identity.Queries.GetMyProfile;

/// <summary>
/// The <c>data</c> payload of both <c>GET /api/v1/me</c> and <c>PUT /api/v1/me</c>
/// — the sanctioned GET/PUT shared-representation case (Naming §0.1).
/// </summary>
/// <remarks>
/// <c>Assignments</c> is plural but holds a single object, which normally violates
/// Naming §3.4. It is kept because API Contract §2 fixes the wire field as
/// <c>assignments</c> and states it is "two nullable scalars, not an array".
/// The contract wins over the style rule.
/// </remarks>
public sealed record MyProfileResponse(
    Guid Id,
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? Bio,
    string? ProfilePictureUrl,
    GlobalRole GlobalRole,
    TrackAssignmentDto Assignments);

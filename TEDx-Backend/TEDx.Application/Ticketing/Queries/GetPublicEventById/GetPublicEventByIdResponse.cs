using TEDx.Application.Common.Dtos;
namespace TEDx.Application.Ticketing.Queries.GetPublicEventById
{
    public sealed record GetPublicEventByIdResponse(
    Guid Id,
    string titleEn,
    string titleAr,
    string descriptionEn,
    string descriptionAr,
    DateTime StartsAtUtc,
    DateTime EndsAtUtc,
    string Location,
    string? ImageUrl,
    int Capacity,
    int RemainingSeats,
    string Status,
    MoneyDto TicketPrice,
    int? MaxIndividualQtyPerOrder,
    IReadOnlyList<PackageResponse> Packages
);
}

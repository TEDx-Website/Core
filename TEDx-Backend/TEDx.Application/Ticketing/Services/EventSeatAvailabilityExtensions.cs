namespace TEDx.Application.Ticketing.Services;

public static class EventSeatAvailabilityExtensions
{
    public static int RemainingSeatsFor(
        this IReadOnlyDictionary<Guid, EventSeatAvailability> availability,
        Guid eventId)
        => availability.TryGetValue(eventId, out var seats)
            ? seats.RemainingSeats
            : 0;
}

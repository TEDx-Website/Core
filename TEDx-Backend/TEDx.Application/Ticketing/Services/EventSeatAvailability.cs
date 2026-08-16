namespace TEDx.Application.Ticketing.Availability;

public readonly record struct EventSeatAvailability(Guid EventId, int Capacity, int ConsumedSeats)
{
    public int RemainingSeats => Math.Max(0, Capacity - ConsumedSeats);

    public bool IsSoldOut => RemainingSeats == 0;

    public static EventSeatAvailability Empty(Guid eventId, int capacity)
        => new(eventId, capacity, 0);
}

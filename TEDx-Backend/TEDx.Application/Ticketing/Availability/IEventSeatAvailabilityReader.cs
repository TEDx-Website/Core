namespace TEDx.Application.Ticketing.Availability;

public interface IEventSeatAvailabilityReader
{
    Task<EventSeatAvailability?> GetAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<Guid, EventSeatAvailability>> GetManyAsync(
        IReadOnlyCollection<Guid> eventIds,
        CancellationToken cancellationToken = default);
}

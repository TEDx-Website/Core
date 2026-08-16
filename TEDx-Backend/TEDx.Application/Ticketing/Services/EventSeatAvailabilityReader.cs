using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Services;

public sealed class EventSeatAvailabilityReader : IEventSeatAvailabilityReader
{
    private readonly IApplicationDbContext _db;
    private readonly IClock _clock;

    public EventSeatAvailabilityReader(IApplicationDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<EventSeatAvailability?> GetAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var row = await _db.Events
            .AsNoTracking()
            .Where(e => e.Id == eventId)
            .Select(ProjectSeats(_clock.UtcNow))
            .SingleOrDefaultAsync(cancellationToken);

        return row is null
            ? null
            : new EventSeatAvailability(row.EventId, row.Capacity, row.ConsumedSeats ?? 0);
    }

    public async Task<IReadOnlyDictionary<Guid, EventSeatAvailability>> GetManyAsync(
        IReadOnlyCollection<Guid> eventIds,
        CancellationToken cancellationToken = default)
    {
        if (eventIds.Count == 0)
        {
            return new Dictionary<Guid, EventSeatAvailability>();
        }

        var rows = await _db.Events
            .AsNoTracking()
            .Where(e => eventIds.Contains(e.Id))
            .Select(ProjectSeats(_clock.UtcNow))
            .ToListAsync(cancellationToken);

        return rows.ToDictionary(
            row => row.EventId,
            row => new EventSeatAvailability(row.EventId, row.Capacity, row.ConsumedSeats ?? 0));
    }

    private static Expression<Func<Event, SeatRow>> ProjectSeats(DateTime now)
        => e => new SeatRow(
            e.Id,
            e.Capacity,
            e.Orders!
                .Where(o => o.Status == OrderStatus.Paid
                            || (o.Status == OrderStatus.PendingPayment && o.HoldExpiresAtUtc > now))
                .Sum(o => (int?)o.Quantity));

    private sealed record SeatRow(Guid EventId, int Capacity, int? ConsumedSeats);
}

using TEDx.Application.Common.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Errors;
using TEDx.Application.Common.Interfaces;
using TEDx.Application.Ticketing.Services;
using TEDx.Application.Ticketing.Dtos;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.UpdateEvent
{
    public sealed class UpdateEventCommandHandler(
        IApplicationDbContext dbContext,
        IEventSeatAvailabilityReader seatAvailabilityReader)
        : IRequestHandler<UpdateEventCommand, Result<UpdateEventResponse>>
    {
        public async Task<Result<UpdateEventResponse>> Handle(UpdateEventCommand request, CancellationToken ct)
        {
            var eventEntity = await dbContext.Events
                .FirstOrDefaultAsync(x => x.Id == request.EventId, ct);

            if (eventEntity is null)
                return Result<UpdateEventResponse>.Failure(CommonErrors.NotFound);

            // Check the client's RowVersion before saving.
            // Normally, EF Core's optimistic concurrency check during SaveChanges()
            // can detect a stale RowVersion when an UPDATE is executed.
            // However, if the request does not actually change any tracked property,
            // EF Core may skip the UPDATE entirely, which means the database concurrency
            // check would never run. In that case, an outdated request could incorrectly
            // be treated as successful.
            //
            // Example:
            // Admin B read the event when RowVersion = 7.
            // Admin A then updated the event, so the database RowVersion became 8.
            // If Admin B sends the same values he already has with RowVersion = 7,
            // EF Core may detect no changes and skip the UPDATE. Without this check,
            // the stale request could incorrectly be treated as successful.
            //
            // This explicit check makes the behavior deterministic: if the client is
            // working with an outdated version of the event, reject the request even when
            // the requested values are identical to the current values.
            if (!eventEntity.RowVersion.SequenceEqual(request.RowVersion))
                return Result<UpdateEventResponse>.Failure(CommonErrors.ConcurrencyConflict);

            var availability = await seatAvailabilityReader.GetAsync(request.EventId, ct);

            if (availability is not null && request.Capacity < availability.Value.ConsumedSeats)
                return Result<UpdateEventResponse>.Failure(TicketingErrors.CapacityBelowSold);

            dbContext.Entry(eventEntity)
                .Property(x => x.RowVersion)
                .OriginalValue = request.RowVersion;

            eventEntity.TitleEn = request.TitleEn;
            eventEntity.TitleAr = request.TitleAr;
            eventEntity.DescriptionEn = request.DescriptionEn;
            eventEntity.DescriptionAr = request.DescriptionAr;
            eventEntity.Venue = request.Location;
            eventEntity.StartAtUtc = request.StartsAtUtc;
            eventEntity.EndAtUtc = request.EndsAtUtc;
            eventEntity.Capacity = request.Capacity;
            eventEntity.TicketPrice = request.TicketPrice.Amount;
            eventEntity.MaxIndividualQtyPerOrder = request.MaxIndividualQtyPerOrder;

            try
            {
                await dbContext.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<UpdateEventResponse>.Failure(CommonErrors.ConcurrencyConflict);
            }

            // eventEntity.RowVersion now holds the value the database generated for this UPDATE,
            // so the client can chain another edit without a round trip.
            var dto = new UpdateEventResponse(
                Id: eventEntity.Id,
                TitleEn: request.TitleEn,
                TitleAr: request.TitleAr,
                DescriptionEn: request.DescriptionEn,
                DescriptionAr: request.DescriptionAr,
                StartsAtUtc: request.StartsAtUtc,
                EndsAtUtc: request.EndsAtUtc,
                Location: request.Location,
                Capacity: request.Capacity,
                TicketPrice: new MoneyDto(eventEntity.TicketPrice, request.TicketPrice.Currency),
                MaxIndividualQtyPerOrder: request.MaxIndividualQtyPerOrder,
                Status: eventEntity.Status,
                RowVersion: Convert.ToBase64String(eventEntity.RowVersion));

            return Result<UpdateEventResponse>.Success(dto);
        }
    }
}

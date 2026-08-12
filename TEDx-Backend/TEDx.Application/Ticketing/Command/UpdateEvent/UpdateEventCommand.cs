using System;
using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Application.Ticketing.DTOs;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Command.UpdateEvent
{
    public sealed record UpdateEventCommand(
        Guid EventId,
        string TitleEn,
        string TitleAr,
        string DescriptionEn,
        string DescriptionAr,
        string Venue,
        DateTime StartsAtUtc,
        DateTime EndsAtUtc,
        int Capacity,
        MoneyDto TicketPrice,
        int? MaxIndividualQtyPerOrder,
        byte[] RowVersion
    ) : IRequest<Unit>, IRequireAdmin;
}

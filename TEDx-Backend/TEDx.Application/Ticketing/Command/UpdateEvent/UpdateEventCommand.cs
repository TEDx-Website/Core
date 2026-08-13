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
        DateTime StartsAtUtc,
        DateTime EndsAtUtc,
        string Location,
        int Capacity,
        MoneyDto TicketPrice,
        int? MaxIndividualQtyPerOrder,
        byte[] RowVersion
    ) : IRequest<Result<UpdateEventDTO>>, IRequireAdmin;
}

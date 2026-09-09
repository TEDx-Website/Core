using TEDx.Application.Common.Dtos;
using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Application.Ticketing.Dtos;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.UpdateEvent
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
        string? ImageUrl,
        byte[] RowVersion
    ) : IRequest<Result<UpdateEventResponse>>, IRequireAdmin;
}

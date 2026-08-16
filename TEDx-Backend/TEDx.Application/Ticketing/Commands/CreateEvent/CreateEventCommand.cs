using TEDx.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;
using TEDx.Application.Ticketing.Dtos;

namespace TEDx.Application.Ticketing.Commands.CreateEvent
{
    public sealed record CreateEventCommand(
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
        string? ImageUrl
    ) : IRequest<Result<CreateEventResponse>>, IRequireAdmin;
}

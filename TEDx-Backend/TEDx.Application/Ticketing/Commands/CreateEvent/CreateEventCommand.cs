using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;
using TEDx.Application.Ticketing.DTOs;

namespace TEDx.Application.Ticketing.Command.CreateEvents
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
    ) : IRequest<Result<CreateEventDTO>>, IRequireAdmin;
}

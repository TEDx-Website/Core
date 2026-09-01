using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Application.Ticketing.Dtos;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.CancelEvent
{
    public sealed record CancelEventCommand(
        Guid id
        ) : IRequest<Result<CancelEventResponse>> , IRequireAdmin;
}

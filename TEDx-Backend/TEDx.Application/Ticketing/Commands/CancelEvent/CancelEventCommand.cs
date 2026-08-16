using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Application.Ticketing.DTOs;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Command.CancelEvent
{
    public sealed record CancelEventCommand(
        Guid id
        ) : IRequest<Result<CancelEventResponse>> , IRequireAdmin;
}

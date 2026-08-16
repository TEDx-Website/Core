using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Application.Ticketing.DTOs;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Command.ChangeEventStatus
{
    public sealed record ChangeEventStatusCommand(
    Guid Id,
    EventStatus TargetStatus
) : IRequest<Result<ChangeEventStatusDTO>>, IRequireAdmin;
}

using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.Commands.ChangeEventStatus
{
    public sealed record ChangeEventStatusCommand(
    Guid Id,
    EventStatus TargetStatus
) : IRequest<Result<ChangeEventStatusResponse>>, IRequireAdmin;
}

using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;
using TEDx.Domain.Communication.Enums;
namespace TEDx.Application.Ticketing.Commands.UpdateContactStatus
{
    public sealed record UpdateContactStatusCommand(
   Guid Id,
   ContactStatus Status
) : IRequest<Result<UpdateContactStatusResponse>>, IRequireAdmin;
}

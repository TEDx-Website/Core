using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;
namespace TEDx.Application.Ticketing.Command.DeleteEvent
{
    public class DeleteEventCommand : IRequest<Result<Unit>>, IRequireAdmin
    {
       public Guid EventId { get; set; }
    }
}

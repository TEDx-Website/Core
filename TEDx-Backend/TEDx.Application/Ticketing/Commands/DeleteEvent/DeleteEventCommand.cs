using MediatR;
using TEDx.Application.Common.Interfaces.Authorization;
using TEDx.Domain.Common;

namespace TEDx.Application.Ticketing.Commands.DeleteEvent;

public sealed record DeleteEventCommand(Guid EventId) : IRequest<Result<Unit>>, IRequireAdmin;

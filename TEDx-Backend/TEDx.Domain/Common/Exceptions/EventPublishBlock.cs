namespace TEDx.Domain.Common.Exceptions;

/// <summary>
/// Why an event failed its publish precondition. Read by the Application layer to
/// pick the outbound error code, so each member is a distinct, mappable situation.
/// </summary>
public enum EventPublishBlock
{
    InvalidTicketPrice = 0,
    InvalidCapacity = 1
}

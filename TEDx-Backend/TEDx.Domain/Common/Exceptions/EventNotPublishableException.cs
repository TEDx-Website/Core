namespace TEDx.Domain.Common.Exceptions;

public sealed class EventNotPublishableException : DomainException
{
    public EventPublishBlock Block { get; }

    public EventNotPublishableException(EventPublishBlock block, string reason)
        : base($"Event cannot be published: {reason}")
    {
        Block = block;
    }
}

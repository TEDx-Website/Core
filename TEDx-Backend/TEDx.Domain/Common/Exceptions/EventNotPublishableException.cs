namespace TEDx.Domain.Common.Exceptions
{
    public sealed class EventNotPublishableException : DomainException
    {
        public EventNotPublishableException(string reason)
            : base($"Event cannot be published: {reason}")
        {
        }
    }
}

namespace TEDx.Domain.Common.Exceptions
{
    public sealed class EventHasOrdersException : DomainException
    {
        public EventHasOrdersException()
            : base("Cannot revert a published event that already has orders.")
        {
        }
    }
}
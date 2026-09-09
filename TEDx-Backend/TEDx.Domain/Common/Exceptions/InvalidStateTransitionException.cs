using System;

namespace TEDx.Domain.Common.Exceptions
{
    public sealed class InvalidStateTransitionException : DomainException
    {
        public InvalidStateTransitionException(string aggregate, object from, object to)
            : base($"{aggregate} cannot transition from '{from}' to '{to}'.")
        {
        }
    }
}

using System;

namespace TEDx.Domain.Common.Exceptions
{
    /// <summary>
    /// Thrown when a status change is attempted from a source state that does
    /// not permit the requested target state (D:Q55 state-machine invariant).
    /// </summary>
    public sealed class InvalidStateTransitionException : DomainException
    {
        public InvalidStateTransitionException(string aggregate, object from, object to)
            : base($"{aggregate} cannot transition from '{from}' to '{to}'.")
        {
        }
    }
}

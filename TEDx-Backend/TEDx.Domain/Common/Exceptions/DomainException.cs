using System;

namespace TEDx.Domain.Common.Exceptions
{
    /// <summary>
    /// Base type for all domain-rule violations. Thrown by aggregates when an
    /// operation would break an invariant (e.g. an illegal status transition).
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}

using System;
using TEDx.Domain.Common.DomainInterfaces;

namespace TEDx.Domain.Common.Entities
{
    /// <summary>
    /// Base class for non-Identity aggregates that carry audit metadata.
    /// Implements <see cref="IAuditable"/> once so entities do not repeat the
    /// four audit properties. The <c>AuditInterceptor</c> stamps these via the
    /// polymorphic <c>ChangeTracker.Entries&lt;IAuditable&gt;()</c> query.
    /// The Identity <c>User</c> entity cannot inherit this (it already inherits
    /// <c>IdentityUser&lt;Guid&gt;</c>) and implements <see cref="IAuditable"/> directly.
    /// </summary>
    public abstract class AuditableEntity : IAuditable
    {
        public DateTime CreatedAtUtc { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
    }
}

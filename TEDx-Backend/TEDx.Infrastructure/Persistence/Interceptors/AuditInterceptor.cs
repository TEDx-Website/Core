using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Common.Abstractions;

namespace TEDx.Infrastructure.Persistence.Interceptors
{
    public sealed class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUser _currentUser;
        private readonly IClock _clock;

        public AuditInterceptor(
            ICurrentUser currentUser,
            IClock clock)
        {
            _currentUser = currentUser;
            _clock = clock;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            UpdateAuditFields(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateAuditFields(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateAuditFields(DbContext? context)
        {
            if (context is null)
                return;

            var now = _clock.UtcNow;
            var userId = _currentUser.UserId?.ToString(); // convert Guid? -> string?

            foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAtUtc = now;

                    // Only IsDeleted/DeletedAtUtc (and whatever the audit loop below
                    // sets) should be written. Without this, EF marks every column
                    // on the row as modified, and the UPDATE statement overwrites
                    // any other property with whatever was loaded in memory —
                    // risking a lost update if another request changed the row
                    // concurrently.
                    //foreach (var property in entry.Properties)
                    //{
                    //    var name = property.Metadata.Name;
                    //    if (name != nameof(ISoftDeletable.IsDeleted)
                    //        && name != nameof(ISoftDeletable.DeletedAtUtc))
                    //    {
                    //        property.IsModified = false;
                    //    }
                    //}
                }
            }

            foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = now;
                    entry.Entity.CreatedBy = userId;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc = now;
                    entry.Entity.UpdatedBy = userId;
                }
            }
        }
    }
}

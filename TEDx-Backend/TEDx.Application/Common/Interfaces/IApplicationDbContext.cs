using TEDx.Domain.Communication.Entities;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using TEDx.Domain.Outbox;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Training.Entities;
using static System.Collections.Specialized.BitVector32;

namespace TEDx.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Identity
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    // Eventing
    DbSet<Event> Events { get; }
    DbSet<Order> Orders { get; }
    DbSet<Package> Packages { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<PromoCode> PromoCodes { get; }
    DbSet<Payment> Payments { get; }
    DbSet<RefundEntry> RefundEntries { get; }

    // Training
    DbSet<Track> Tracks { get; }
    DbSet<Session> Sessions { get; }
    DbSet<TrackAssignment> TrackAssignments { get; }
    DbSet<Attendance> Attendances { get; }
    DbSet<Evaluation> Evaluations { get; }

    // Notifications
    DbSet<Notification> Notifications { get; }
    DbSet<NotificationRecipient> NotificationRecipients { get; }
    DbSet<ContactMessage> ContactMessages { get; }
    DbSet<OutboxMessage> OutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

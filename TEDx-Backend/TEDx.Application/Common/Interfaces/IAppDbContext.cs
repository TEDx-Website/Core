using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TEDx.Domain.Communication;
using TEDx.Domain.Cross_Cutting;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Training.Entities;
using static System.Collections.Specialized.BitVector32;

namespace TEDx.Application.Common.Interfaces;

public interface IAppDbContext
{
    // Identity
    DbSet<User> ApplicationUsers { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    // Eventing
    DbSet<Event> Events { get; }
    DbSet<Order> Orders { get; }
    DbSet<Package> Packages { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<PromoCode> PromoCodes { get; }
    DbSet<Payment> Payments { get; }

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
    DbSet<OutOfBokMessages> OutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
}

using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
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
    DbSet<Packages> Packages { get; }
    DbSet<Tickets> Tickets { get; }

    // Training
    DbSet<Track> Tracks { get; }
    DbSet<Sessions> Sessions { get; }
    DbSet<TrackAssignment> TrackAssignments { get; }
    DbSet<Attendence> Attendances { get; }
    DbSet<Evaluation> Evaluations { get; }

    // Notifications
    DbSet<Notification> Notifications { get; }
    DbSet<NotificationRecepient> NotificationRecipients { get; }
    DbSet<ContactMessage> ContactMessages { get; }
    DbSet<OutOfBokMessages> OutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Communication;
using TEDx.Domain.Cross_Cutting;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Training.Entities;

namespace TEDx.Infrastructure.Persistence;

public sealed class AppDbContext
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // DbSets — expression-bodied so EF backs them via Set<T>()

    // Identity
    public DbSet<User> ApplicationUsers => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Eventing
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Packages> Packages => Set<Packages>();
    public DbSet<Tickets> Tickets => Set<Tickets>();

    // Training
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<Sessions> Sessions => Set<Sessions>();
    public DbSet<TrackAssignment> TrackAssignments => Set<TrackAssignment>();
    public DbSet<Attendence> Attendances => Set<Attendence>();
    public DbSet<Evaluation> Evaluations => Set<Evaluation>();

    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRecepient> NotificationRecipients => Set<NotificationRecepient>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<OutOfBokMessages> OutboxMessages => Set<OutOfBokMessages>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}

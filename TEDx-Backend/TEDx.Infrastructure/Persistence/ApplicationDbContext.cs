using Microsoft.EntityFrameworkCore;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Communication;
using TEDx.Domain.Cross_Cutting;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Training.Entities;

namespace TEDx.Infrastructure.Persistence;

public sealed class AppDbContext
    : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // DbSets

    // Identity
    public DbSet<ApplicationUser> ApplicationUsers {  get;}
    public DbSet<RefreshToken> RefreshTokens {  get;}

    // Eventing
    public DbSet<Event> Events { get; }
    public DbSet<Order> Orders { get; }
    public DbSet<Packages> Packages { get; }
    public DbSet<Tickets> Tickets { get; }

    // Training
    public DbSet<Track> Tracks { get; }
    public DbSet<Sessions> Sessions { get; }
    public DbSet<TrackAssignment> TrackAssignments { get; }
    public DbSet<Attendence> Attendances { get; }
    public DbSet<Evaluation> Evaluations { get; }

    // Notifications
    public DbSet<Notification> Notifications { get; }
    public DbSet<NotificationRecepient> NotificationRecipients { get; }
    public DbSet<ContactMessage> ContactMessages { get; }
    public DbSet<OutOfBokMessages> OutboxMessages { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}

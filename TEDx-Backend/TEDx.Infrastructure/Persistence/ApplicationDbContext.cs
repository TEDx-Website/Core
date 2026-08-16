using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TEDx.Application.Common.Interfaces;
using TEDx.Domain.Communication;
using TEDx.Domain.Cross_Cutting;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Training.Entities;
namespace TEDx.Infrastructure.Persistence;

public sealed class ApplicationDbContext
    : IdentityUserContext<User,Guid>, IAppDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<RefundEntry> RefundEntries => Set<RefundEntry>();
    // Training
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<TrackAssignment> TrackAssignments => Set<TrackAssignment>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Evaluation> Evaluations => Set<Evaluation>();
    // Notifications
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRecipient> NotificationRecipients => Set<NotificationRecipient>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<OutOfBokMessages> OutboxMessages => Set<OutOfBokMessages>();

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => Database.BeginTransactionAsync(cancellationToken);


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<IdentityUserClaim<Guid>>();
        modelBuilder.Ignore<IdentityUserLogin<Guid>>();
        modelBuilder.Ignore<IdentityUserToken<Guid>>();

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}

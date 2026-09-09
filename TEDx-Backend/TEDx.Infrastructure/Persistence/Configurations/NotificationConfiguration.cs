using TEDx.Domain.Communication.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Training.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Body)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.AudienceType)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne<Track>().WithMany()
              .HasForeignKey(x => x.TrackId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

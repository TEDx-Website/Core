using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Communication;
using TEDx.Domain.Training.Entities;
using TEDx.Domain.Training.Enums;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("Sessions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TitleEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.TitleAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Location)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .HasDefaultValue(SessionStatus.Scheduled);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Track).WithMany(t => t.Sessions)
                .HasForeignKey(x => x.TrackId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

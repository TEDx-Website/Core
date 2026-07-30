using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TitleEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.TitleAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.DescriptionEn)
                .HasMaxLength(3000)
                .IsRequired();

            builder.Property(x => x.DescriptionAr)
                .HasMaxLength(3000)
                .IsRequired();

            builder.Property(x => x.Venue)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(x => x.StartAtUtc)
               .IsRequired();

            builder.Property(x => x.EndAtUtc)
               .IsRequired();

            builder.Property(x => x.TicketPrice)
                .HasPrecision(18, 2);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }

      
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Training.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class TrackConfiguration : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            builder.ToTable("Tracks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.NameAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.DescriptionEn)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.DescriptionAr)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Schedule)
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

        }
    }
}

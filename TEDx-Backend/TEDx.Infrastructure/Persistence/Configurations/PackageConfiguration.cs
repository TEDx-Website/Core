using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.ToTable("Packages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.NameAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Price)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.SeatsPerPackage)
                .IsRequired();

            builder.Property(x => x.MaxQuantityPerOrder);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.HasOne(x => x.Event).WithMany(e => e.Packages)
                .HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

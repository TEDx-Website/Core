using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.ToTable("PromoCodes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .HasMaxLength(40)
                .IsRequired();

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.Property(x => x.DiscountType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.DiscountValue)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.MaxTotalRedemption);

            builder.Property(x => x.MaxPerUser);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Event).WithMany(e => e.PromoCodes)
                .HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Restrict);

            // Own soft-delete, plus the matching filter that keeps PromoCodes invisible
            // when their Event is soft-deleted.
            builder.HasQueryFilter(x => !x.IsDeleted && !x.Event.IsDeleted);

        }
    }
}

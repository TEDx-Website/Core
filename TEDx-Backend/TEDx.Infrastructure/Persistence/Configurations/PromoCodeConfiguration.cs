using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCodes>
    {
        public void Configure(EntityTypeBuilder<PromoCodes> builder)
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

            builder.HasOne<Event>().WithMany()
                .HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<Order>().WithOne()
                .HasForeignKey(x => x.PromoCodeId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<PromoRedemption>().WithOne()
               .HasForeignKey(x => x.PromoCodeId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}

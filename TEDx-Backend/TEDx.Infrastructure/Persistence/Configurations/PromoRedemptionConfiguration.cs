using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class PromoRedemptionConfiguration
    : IEntityTypeConfiguration<PromoRedemption>
    {
        public void Configure(EntityTypeBuilder<PromoRedemption> builder)
        {
            builder.ToTable("PromoRedemptions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PromoRedemptionStatus)
                .HasConversion<int>()
                .HasDefaultValue(PromoRedemptionStatus.Active);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<PromoCodes>().WithMany()
                .HasForeignKey(x => x.PromoCodeId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Order>().WithMany()
                .HasForeignKey(x => x.PromoCodeId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}

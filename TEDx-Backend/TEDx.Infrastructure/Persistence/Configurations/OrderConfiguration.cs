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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.AccountId)
               .IsRequired();

            builder.Property(x => x.EventId)
               .IsRequired();

            builder.Property(x => x.OrderReference)
               .IsRequired();

            builder.Property(x => x.UnitType)
               .IsRequired();

            builder.Property(x => x.OrderReference)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(x => x.OrderReference)
                .IsUnique();

            builder.Property(x => x.UnitType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.UnitNameSnapshot)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .IsRequired();
            //builder.HasIndex(x => x.Quantity).HasFilter("")

            builder.Property(x => x.UnitPriceSnapshot)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.SubTotalSnapshot)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.DiscountSnapshot)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m);

            builder.Property(x => x.TotalSnapshot)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.PromoCodeSnapshot)
                .HasMaxLength(40);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .HasDefaultValue(OrderStatus.PendingPayment);
            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasIndex(o => new
            {
                o.EventId,
                o.Status
            })
            .HasDatabaseName("IX_Order_Event_Status");


            builder.HasIndex(o => o.HoldExpiresAtUtc)
                    .HasDatabaseName("IX_Order_HoldExpiry")
                    .HasFilter("[Status] = 0");

            builder.HasOne<User>().WithMany()
                .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Event).WithMany(e => e.Orders)
                .HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Package).WithMany(p => p.Orders)
                .HasForeignKey(x => x.PackageId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PromoCode).WithMany(pc => pc.Orders)
                .HasForeignKey(x => x.PromoCodeId).OnDelete(DeleteBehavior.Restrict);

            // Matching filter: keeps Orders invisible when their Event is soft-deleted.
            builder.HasQueryFilter(x => !x.Event.IsDeleted);
        }
    }
}

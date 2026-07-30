using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payement>
    {
        public void Configure(EntityTypeBuilder<Payement> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PaymobOrderId)
                .HasMaxLength(64);

            builder.Property(x => x.PaymobTransactionId)
                .HasMaxLength(64);

            builder.HasIndex(x => x.PaymobTransactionId)
                .IsUnique();

            builder.Property(x => x.PaymentSessionId)
                .HasMaxLength(128);

            builder.Property(x => x.IdempotencyKey)
                .HasMaxLength(64);

            builder.HasIndex(x => x.IdempotencyKey)
                .IsUnique();

            builder.Property(x => x.PaymentStatus)
                .HasConversion<int>()
                .HasDefaultValue(PaymentStatus.Initiated);

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("EGP");

            builder.Property(x => x.RawPayloadJson)
                .HasColumnType("nvarchar(max)");

            builder.HasOne(x => x.Order).WithMany(o => o.Payements)
                .HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

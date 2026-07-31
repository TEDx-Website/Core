using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class RefundEntryConfiguration
    : IEntityTypeConfiguration<RefundEntry>
    {
        public void Configure(EntityTypeBuilder<RefundEntry> builder)
        {
            builder.ToTable("RefundEntries");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Reason)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.VoidedTicketsCount)
                .IsRequired();

            builder.Property(x => x.SeatsReleased)
                .IsRequired();

            builder.Property(x => x.CheckedInTicketsRetained)
                .IsRequired();

            builder.HasOne(x => x.Order).WithMany(o => o.RetryRefunds)
                .HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);

            // Matching filter: keeps RefundEntry invisible when their Order's Event is soft-deleted.
            builder.HasQueryFilter(x => !x.Order.Event.IsDeleted);
        }
    }
}

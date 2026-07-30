using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Ticketing.Enums;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class TicketConfiguration : IEntityTypeConfiguration<Tickets>
    {
        public void Configure(EntityTypeBuilder<Tickets> builder)
        {
            builder.ToTable("Tickets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TicketReference)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(x => x.TicketReference)
                .IsUnique();

            builder.Property(x => x.QrSecretHash)
                .HasMaxLength(88)
                .IsRequired();

            builder.HasIndex(x => x.QrSecretHash)
                .IsUnique();

            builder.Property(x => x.GuestName)
                .HasMaxLength(200);

            builder.Property(x => x.TicketsStatus)
                .HasConversion<int>()
                .HasDefaultValue(TicketsStatus.Issued);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne(x => x.Event).WithMany(e => e.Tickets)
                .HasForeignKey(x => x.EventId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Order).WithMany(o => o.Tickets)
                .HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

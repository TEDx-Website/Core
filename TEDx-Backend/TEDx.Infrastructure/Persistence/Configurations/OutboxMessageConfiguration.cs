using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Outbox;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class OutboxMessageConfiguration
    : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.PayloadJson)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.Attempts)
                .HasDefaultValue(0);

            builder.Property(x => x.LastError)
                .HasColumnType("nvarchar(max)");
        }
    }
}

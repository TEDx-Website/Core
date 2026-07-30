using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Communication;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class ContactMessageConfiguration
     : IEntityTypeConfiguration<ContactMessage>
    {
        public void Configure(EntityTypeBuilder<ContactMessage> builder)
        {
            builder.ToTable("ContactMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.Subject)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Message)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .HasDefaultValue(ContactStatus.New);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Communication;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Identity.Enums;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Training.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.NormalizedUserName)
                .HasMaxLength(256)
                .IsRequired();
            builder.HasIndex(x => x.NormalizedUserName).IsUnique();

            builder.Property(x => x.Email)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.NormalizedEmail)
                .HasMaxLength(256)
                .IsRequired();
            builder.HasIndex(x => x.NormalizedEmail).IsUnique();

            builder.Property(x => x.EmailConfirmed)
                .IsRequired();

            builder.Property(x => x.SecurityStamp)
                .IsRequired();
            builder.Property(x => x.ConcurrencyStamp)
              .IsRequired();
            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(32);

            builder.Property(x => x.PhoneNumberConfirmed)
                .IsRequired();
            builder.Property(x => x.TwoFactorEnabled)
                .IsRequired();
            builder.Property(x => x.LockoutEnabled)
                .IsRequired();
            builder.Property(x => x.AccessFailedCount)
                .IsRequired();

            builder.Property(x => x.Role)
                .HasConversion<int>()
                .HasDefaultValue(GlobalRole.Attendee)
                .IsRequired();

            builder.Property(x => x.IsActive)
               .HasDefaultValue(true).IsRequired();

            builder.Property(x => x.ProfilePictureUrl)
                .HasMaxLength(500);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }

    }
}

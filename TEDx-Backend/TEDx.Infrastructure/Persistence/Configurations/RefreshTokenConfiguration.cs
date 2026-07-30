using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Identity.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TokenHash)
                .HasMaxLength(88)
                .IsRequired();

            builder.Property(x => x.AccountId)
                .IsRequired();

            builder.Property(x => x.ExpiredAtUTC)
                .IsRequired();

            builder.Property(x => x.ReplaacedByTokenHash)
                .HasMaxLength(88);
            builder.Property(x => x.CreatedBtIp)
                .HasMaxLength(45);

            builder.HasOne<User>().WithMany()
                .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}

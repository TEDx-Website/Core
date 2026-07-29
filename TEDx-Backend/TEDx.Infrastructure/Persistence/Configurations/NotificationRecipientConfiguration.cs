using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Communication;
using TEDx.Domain.Identity.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class NotificationRecipientConfiguration
     : IEntityTypeConfiguration<NotificationRecepient>
    {
        public void Configure(EntityTypeBuilder<NotificationRecepient> builder)
        {
            builder.ToTable("NotificationRecipients");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IsRead)
                .HasDefaultValue(false);

            builder.HasOne<ApplicationUser>().WithMany()
               .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne<Notification>().WithMany()
              .HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}

using TEDx.Domain.Communication.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Identity.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class NotificationRecipientConfiguration
     : IEntityTypeConfiguration<NotificationRecipient>
    {
        public void Configure(EntityTypeBuilder<NotificationRecipient> builder)
        {
            builder.ToTable("NotificationRecipients");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IsRead)
                .HasDefaultValue(false);

            builder.HasOne<User>().WithMany()
               .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Notification).WithMany(n => n.NotificationRecipients)
              .HasForeignKey(x => x.NotificationId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}

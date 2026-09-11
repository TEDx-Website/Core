using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Ticketing.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public class SpeakerConfiguration : IEntityTypeConfiguration<Speaker>
    {
        public void Configure(EntityTypeBuilder<Speaker> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder
                .Property(x => x.PictureUrl)
                .IsRequired();

            builder
                .Property(x => x.TagLine)
                .IsRequired()
                .HasMaxLength(300);

            builder
                .Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000);
        }
    }
}

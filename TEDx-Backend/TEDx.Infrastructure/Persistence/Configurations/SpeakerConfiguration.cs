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
                .Property(x => x.SpeakerName)
                .IsRequired()
                .HasMaxLength(200);

            builder
                .Property(x => x.SpeakerPictureUrl)
                .IsRequired();

            builder
                .Property(x => x.SpeakerBio)
                .IsRequired()
                .HasMaxLength(2000);

            builder
                .Property(x => x.TopSpeakerOrderIndex)
                .IsRequired(false);

            builder
                .Property(x => x.IsNextSpeaker)
                .HasDefaultValue(false);

            builder
                .Property(x => x.SpeakerRole)
                .IsRequired()
                .HasMaxLength(300);

            builder
                .Property(x => x.TalkTrack)
                .IsRequired()
                .HasMaxLength(200);

            builder
                .Property(x => x.TalkTitle)
                .IsRequired()
                .HasMaxLength(300);

            builder
                .Property(x => x.TalkShortDescription)
                .IsRequired()
                .HasMaxLength(1000);

            builder
                .Property(x => x.SpeakerLinkedInUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder
                .Property(x => x.SpeakerXUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder
                .Property(x => x.SpeakerWebsiteUrl)
                .HasMaxLength(500)
                .IsRequired(false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Communication;
using TEDx.Domain.Training.Entities;
using TEDx.Domain.Training.Enums;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class SessionConfiguration : IEntityTypeConfiguration<Sessions>
    {
        public void Configure(EntityTypeBuilder<Sessions> builder)
        {
            builder.ToTable("Sessions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TitleEn)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.TitlleAr)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Location)
                .HasMaxLength(300)
                .IsRequired();

            //builder.Property(x => x.SessionStatus)
            //    .HasConversion<int>()
            //    .HasDefaultValue(SessionStatus.);

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne<Track>().WithMany()
                .HasForeignKey(x => x.TrackId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<Attendence>().WithOne()
              .HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<Evaluation>().WithOne()
              .HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

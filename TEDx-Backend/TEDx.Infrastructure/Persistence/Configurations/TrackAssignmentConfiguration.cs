using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Identity.Entities;
using TEDx.Domain.Ticketing.Entities;
using TEDx.Domain.Training.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class TrackAssignmentConfiguration
    : IEntityTypeConfiguration<TrackAssignment>
    {
        public void Configure(EntityTypeBuilder<TrackAssignment> builder)
        {
            builder.ToTable("TrackAssignments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TrackRole)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();
            builder.HasOne<User>().WithMany()
                .HasForeignKey(x => x.AccountId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Track>().WithMany()
               .HasForeignKey(x => x.TrackId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<Attendence>().WithOne()
            .HasForeignKey(x => x.EnrollmentId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany<Evaluation>().WithOne()
            .HasForeignKey(x => x.EnrollmentId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

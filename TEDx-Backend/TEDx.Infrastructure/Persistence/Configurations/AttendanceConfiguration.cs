using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TEDx.Domain.Training.Entities;

namespace TEDx.Infrastructure.Persistence.Configurations
{
    public sealed class AttendanceConfiguration
    : IEntityTypeConfiguration<Attendence>
    {
        public void Configure(EntityTypeBuilder<Attendence> builder)
        {
            builder.ToTable("Attendances");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.RowVersion)
                .IsRowVersion();

            builder.HasOne<Sessions>().WithMany()
               .HasForeignKey(x => x.SessionId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<TrackAssignment>().WithMany()
               .HasForeignKey(x => x.EnrollmentId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

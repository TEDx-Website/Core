using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Training.Enums;

namespace TEDx.Domain.Training.Entities
{
    public class TrackAssignment : AuditableEntity, IConcurrent
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid TrackId { get; set; }
        public TrackRole TrackRole { get; set; }
        public DateTime StartAtUtc { get; set; }
        public DateTime? EndAtUtc { get; set; }
        public Guid AssignedBy { get; set; }
        public Guid? EndedBy { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        public Track Track { get; set; } = null!;
        public List<Attendance>? Attendances { get; set; }
        public List<Evaluation>? Evaluations { get; set; }
    }
}

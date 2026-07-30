using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;
using TEDx.Domain.Training.Enums;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;

namespace TEDx.Domain.Training.Entities
{
    public class Attendance : AuditableEntity, IConcurrent
    {
        public Guid Id { get; private set; }

        public Guid SessionId { get; private set; }
        public Guid EnrollmentId { get; private set; }

        public AttendanceStatus Status { get; private set; }

        public DateTime? RecordedAtUtc { get; private set; }
        public Guid? RecordedBy { get; private set; }
        public byte[] RowVersion { get; set; } = null!;
        public TrackAssignment TrackAssignment { get; set; } = null!;
        public Session Session { get; set; } = null!;
    }
}

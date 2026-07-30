using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;
using TEDx.Domain.Training.Enums;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;

namespace TEDx.Domain.Training.Entities
{
    public class  Attendence : AuditableEntity , IConcurrent
    {
        public Guid Id { get; private set; }

        public Guid SessionId { get; private set; } // nn
        public Guid EnrollmentId { get; private set; }// nn

        public AttendanceStatus Status { get; private set; }//nn

        public DateTime? RecordedAtUtc { get; private set; }//n
        public Guid? RecordedBy { get; private set; }//nn
        public byte[] RowVersion { get; set; }// nn
        public TrackAssignment TrackAssignment { get; set; }
        public Sessions Session {  get; set; }
    }
}

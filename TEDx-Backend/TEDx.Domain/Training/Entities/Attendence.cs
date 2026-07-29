using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;
using TEDx.Domain.Training.Enums;
using TEDx.Domain.Common.DomainInterfaces;

namespace TEDx.Domain.Training.Entities
{
    public class  Attendence : IAuditable , IConcurrent
    {
        public Guid Id { get; private set; }

        public Guid SessionId { get; private set; } // nn
        public Guid EnrollmentId { get; private set; }// nn

        public AttendeceStatus Status { get; private set; }//nn

        public DateTime? RecordedAtUtc { get; private set; }//n
        public Guid? RecordedBy { get; private set; }//nn
        public DateTime CreatedAtUtc { get; set; }//nn
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; }// nn
        public TrackAssignment TrackAssignment { get; set; }
        public Sessions Session {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;

namespace TEDx.Domain.Training.Entities
{
    public class Evaluation : IAuditable , IConcurrent
    {
        public Guid Id { get; private set; }

        public Guid SessionId { get; private set; } //n
        public Guid EnrollmentId { get; private set; }// nn
        public int Score { get; private set; }// nn ck
        public string? Feedback { get; private set; }
        public Guid EvaluatedBy { get; private set; }// nn
        public DateTime CreatedAtUtc { get; set; }//nn
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; }// nn
        public TrackAssignment TrackAssignment { get; set; }
        public Sessions Session { get; set; }

    }
}

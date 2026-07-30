using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;

namespace TEDx.Domain.Training.Entities
{
    public class Evaluation : AuditableEntity , IConcurrent
    {
        public Guid Id { get; private set; }

        public Guid SessionId { get; private set; } //n
        public Guid EnrollmentId { get; private set; }// nn
        public int Score { get; private set; }// nn ck
        public string? Feedback { get; private set; }
        public Guid EvaluatedBy { get; private set; }// nn
        public byte[] RowVersion { get; set; }// nn
        public TrackAssignment TrackAssignment { get; set; }
        public Sessions Session { get; set; }

    }
}

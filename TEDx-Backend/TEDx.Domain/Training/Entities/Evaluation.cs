using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.Abstractions;
using TEDx.Domain.Common.Entities;

namespace TEDx.Domain.Training.Entities
{
    public class Evaluation : AuditableEntity, IHasRowVersion
    {
        public Guid Id { get; private set; }

        public Guid SessionId { get; private set; }
        public Guid EnrollmentId { get; private set; }
        public int Score { get; private set; }
        public string? Feedback { get; private set; }
        public Guid EvaluatedBy { get; private set; }
        public byte[] RowVersion { get; set; } = null!;
        public TrackAssignment TrackAssignment { get; set; } = null!;
        public Session Session { get; set; } = null!;
    }
}

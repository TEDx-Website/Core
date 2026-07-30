using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Training.Enums;

namespace TEDx.Domain.Training.Entities
{
    public class Session : AuditableEntity, ISoftDelete, IConcurrent
    {
        public Guid Id { get; set; }
        public Guid TrackId { get; set; }
        public string? TitleEn { get; set; }
        public string? TitleAr { get; set; }
        public string? Description { get; set; }
        public DateTime StartAtUtc { get; set; }
        public DateTime EndedAtUtc { get; set; }
        public string? Location { get; set; }
        public SessionStatus Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        public Track Track { get; set; } = null!;
        public List<Attendance>? Attendances { get; set; }
        public List<Evaluation>? Evaluations { get; set; }
    }
}

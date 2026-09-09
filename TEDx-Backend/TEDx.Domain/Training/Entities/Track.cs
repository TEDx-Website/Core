using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.Abstractions;
using TEDx.Domain.Common.Entities;

namespace TEDx.Domain.Training.Entities
{
    public class Track : AuditableEntity, ISoftDeletable, IHasRowVersion
    {
        public Guid Id { get; set; }
        public string? NameEn { get; set; }
        public string? NameAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? Schedule { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        public List<Session>? Sessions { get; set; }
        public List<TrackAssignment>? TrackAssignments { get; set; }
    }
}

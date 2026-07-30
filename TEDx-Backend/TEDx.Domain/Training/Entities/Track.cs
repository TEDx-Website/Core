using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;

namespace TEDx.Domain.Training.Entities
{
    public class Track : AuditableEntity , ISoftDelete, IConcurrent
    {
        public Guid Id { get; set; }
        public string? NameEn {  get; set; }// nn FUQ
        public string? NameAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? Schedle {  get; set; }// 500
        public bool IsActive { get; set; } // NN DF
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; }
        List<Sessions> Sessions { get; set; }
        List<TrackAssignment> TrackAssignments { get; set; }
    }
}

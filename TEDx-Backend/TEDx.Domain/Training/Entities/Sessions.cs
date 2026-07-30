using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Training.Enums;

namespace TEDx.Domain.Training.Entities
{
    public class Sessions :IAuditable , ISoftDelete,IConcurrent
    {
        public Guid Id { get; set; }
        public Guid TrackId { get; set; } // nn
        public string? TitleEn {  get; set; }// nn 200
        public string? TitlleAr {  get; set; }// nn 200
        public string? Description {  get; set; } // max
        public DateTime StartAtUtc {  get; set; }// nn
        public DateTime EndedAtUtc { get; set; }// nn
        public string? Location { get; set; } // 300
        public SessionStatus SessionStatus { get; set; } // nn df
        public DateTime CreatedAtUtc { get; set; } // nn
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } // nn df
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; } // nn
        public Track Track { get; set; }
        public List<Attendence> Attendences { get; set; }
        public List<Evaluation> Evaluations { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Training.Enums;

namespace TEDx.Domain.Training.Entities
{
    public class Sessions :AuditableEntity , ISoftDelete,IConcurrent
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
        public bool IsDeleted { get; set; } // nn df
        public DateTime? DeletedAtUtc { get; set; }
        public byte[] RowVersion { get; set; } // nn
        public Track Track { get; set; }
        public List<Attendence> Attendences { get; set; }
        public List<Evaluation> Evaluations { get; set; }
    }
}

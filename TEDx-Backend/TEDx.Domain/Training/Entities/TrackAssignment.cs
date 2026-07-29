using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Training.Enums;

namespace TEDx.Domain.Training.Entities
{
    public class TrackAssignment : IAuditable, IConcurrent
    {
        public Guid Id {  get; set; } //
        public Guid AccountId { get; set; }// nnn
        public Guid TrackId { get; set; } // NN
        public TrackRole TrackRole { get; set; }//nn
        public DateTime StartAtUtc { get; set; }//nn
        public DateTime EndAtUtc { get; set; }
        public DateTime AssignedBy {  get; set; }// NN
        public DateTime EndedBy { get; set; }   
        public DateTime CreatedAtUtc { get; set; }//nn
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; }// nn
        public Track Track { get; set; }
        public List<Attendence> Attendences { get; set; }
        public List<Evaluation> Evaluations { get; set; }
    }
}

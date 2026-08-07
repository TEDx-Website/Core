using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Identity.DTOs
{
    public class TrackAssignmentDTO
    {
        public Guid MemberOfTrackId { get; set; }
        public Guid BoardOfTrackId { get; set; }

    }
}

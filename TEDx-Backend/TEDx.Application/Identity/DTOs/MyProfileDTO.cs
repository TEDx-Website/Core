using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Identity.Enums;
using TEDx.Domain.Training.Entities;

namespace TEDx.Application.Identity.DTOs
{
    public class MyProfileDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Bio { get; set; }
        public string ProfilePictureUrl { get; set; }
        public GlobalRole Role { get; set; }
        public TrackAssignmentDTO Assignments { get; set; } = new();
    }
}

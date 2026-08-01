using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Identity.Enums;
namespace TEDx.Domain.Identity.Entities
{
    public class User : IdentityUser<Guid> , IAuditable , ISoftDelete
    {
        public string? FirstName { get; set; } // 100
        public string? LastName { get; set; }// 100
        public string? Bio { get; set; }// 1000
        public string? ProfilePictureUrl { get; set; }// 500
        public GlobalRole Role { get; set; }// NN Default = attendee
        public bool IsActive { get; set; }// 1
        public DateTime CreatedAtUtc { get; set; } // nn
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } // nn dft
        public DateTime? DeletedAtUtc { get; set; }
        public List<RefreshToken> RefreshTokens {  get; set; }
    }
}

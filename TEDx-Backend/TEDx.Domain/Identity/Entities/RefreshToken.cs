using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Identity.Enums;
namespace TEDx.Domain.Identity.Entities
{
    public class RefreshToken : IRefreshTokenAudit
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string? TokenHash { get; set; }// NN UN varchar88
        public DateTime ExpiredAtUTC { get; set; } //NN
        public string? CreatedBtIp { get; set; } // 45
        public DateTime RevokedAtUtc { get; set; }
        public string? ReplaacedByTokenHash { get; set; }//88
        public ReasonRevoke ReasonRevoked { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}

// APPUSER 1 ---> M REFRESHTOKEN

using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Identity.Enums;
namespace TEDx.Domain.Identity.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string TokenHash { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
        public string? CreatedByIp { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public string? ReplacedByTokenHash { get; set; }
        public ReasonRevoked? ReasonRevoked { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public User ApplicationUser { get; set; } = null!;

        public bool IsActive(DateTime nowUtc) => RevokedAtUtc is null && ExpiresAtUtc > nowUtc;
    }
}

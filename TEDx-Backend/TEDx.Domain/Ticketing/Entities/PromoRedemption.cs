using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class PromoRedemption
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid OrderId { get; set; }
        public Guid PromoCodeId { get; set; }
        public PromoRedemptionStatus Status { get; set; }
        public DateTime ClaimedAtUtc { get; set; }
        public DateTime ReleasedAtUtc { get; set; }
        public DateTime ConfirmedAtUtc { get; set; }
        public Order Order { get; set; } = null!;
        public PromoCode PromoCode { get; set; } = null!;
    }
}

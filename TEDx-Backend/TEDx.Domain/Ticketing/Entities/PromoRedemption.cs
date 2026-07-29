using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class PromoRedemption
    {
        public Guid Id { get; set; }
        public Guid AccountId {  get; set; } // nn
        public Guid OrderId { get; set; } // 
        public Guid PromoCodeId { get; set; } // nn
        public PromoRedemptionStatus PromoRedemptionStatus { get; set; } // nn Df
        public DateTime ClaimedAtUtc { get; set; } // NN
        public DateTime ReleasedAtUtc { get; set; }
        public DateTime ConfirmedAtUtc { get; set; }
        public Order Order { get; set; }
        public PromoCodes PromoCodes { get; set; }
    }
}

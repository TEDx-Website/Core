using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Order :IAuditable, IConcurrent
    {
        public Guid Id {  get; set; }
        public string? OrderReference { get; set; }// UQ NN 200
        public Guid AccountId { get; set; } // NN User1 --> M Orderw
        public Guid EventId { get; set; } // NN Event1 --> M ORders
        public Guid PackageId { get; set; }// Order M --> Package 1
        public OrderUnitType UnitType { get; set; }
        public string? UnitNAmeSnapshot { get; set; }// 200 NN
        public int Quantity { get; set; } // NN Check
        public decimal UnitPriceSnapshot { get; set; } // 18,2
        public decimal SubTotalSnapshot { get; set; } // 18,2
        public decimal DiscountSnapshot { get; set; } // 18,2
        public decimal TotalSnapshot { get; set; } // 18,2
        public Guid PromoCodeId { get; set; }
        public string? PromoCodeSnapshot { get; set; }// 40
        public OrderStatus Status { get; set; } // df NN
        public DateTime HoldExpiresAtUtc {  get; set; }
        public DateTime PAidAtUtc {  get; set; }
        public DateTime CancelledAtUtc { get; set; }
        public DateTime ExpiredAtUtc { get; set; }
        public string? PaymobOrder {  get; set; } // 70
        public DateTime CreatedAtUtc { get; set; }//nn
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; }//nn
        // nav prop
        public Event Event { get; set; }
        public List<Payement>? Payements { get; set; }
        public List<Tickets>? Tickets { get; set; }
        public List<RefundEntry>? RetryRefunds { get; set; }
        public List<PromoRedemption> promoRedemptions { get; set; }
        public Packages Package {  get; set; }
        public PromoCodes PromoCode { get; set; }

    }
}

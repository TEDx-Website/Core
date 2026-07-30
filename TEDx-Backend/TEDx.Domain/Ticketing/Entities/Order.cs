using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.DomainInterfaces;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Common.Exceptions;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Order : AuditableEntity, IConcurrent
    {
        public Guid Id { get; set; }
        public string? OrderReference { get; set; }// UQ NN 200
        public Guid AccountId { get; set; } // NN User1 --> M Orderw
        public Guid EventId { get; set; } // NN Event1 --> M ORders
        public Guid? PackageId { get; set; }// Order M --> Package 1 (nullable: individual-ticket orders have no package)
        public OrderUnitType UnitType { get; set; }
        public string? UnitNameSnapshot { get; set; }// 200 NN
        public int Quantity { get; set; } // NN Check
        public decimal UnitPriceSnapshot { get; set; } // 18,2
        public decimal SubTotalSnapshot { get; set; } // 18,2
        public decimal DiscountSnapshot { get; set; } // 18,2
        public decimal TotalSnapshot { get; set; } // 18,2
        public Guid? PromoCodeId { get; set; } // nullable: an order without a promo has no code
        public string? PromoCodeSnapshot { get; set; }// 40
        public OrderStatus Status { get; private set; } // df NN — changes only via transition methods
        public DateTime HoldExpiresAtUtc { get; set; }
        public DateTime PaidAtUtc { get; private set; }
        public DateTime CancelledAtUtc { get; private set; }
        public DateTime ExpiredAtUtc { get; private set; }
        public string? PaymobOrder { get; set; } // 70
        public byte[] RowVersion { get; set; } = null!;//nn
        // nav prop (intra-context only)
        public Event Event { get; set; } = null!;
        public List<Payement>? Payements { get; set; }
        public List<Tickets>? Tickets { get; set; }
        public List<RefundEntry>? RetryRefunds { get; set; }
        public List<PromoRedemption>? promoRedemptions { get; set; }
        public Packages? Package { get; set; }
        public PromoCodes? PromoCode { get; set; }

        // --- State machine (D:Q55) ---

        /// <summary>PendingPayment → Paid.</summary>
        public void MarkAsPaid(DateTime utcNow)
        {
            if (Status != OrderStatus.PendingPayment)
                throw new InvalidStateTransitionException(nameof(Order), Status, OrderStatus.Paid);

            Status = OrderStatus.Paid;
            PaidAtUtc = utcNow;
        }

        /// <summary>PendingPayment → Cancelled.</summary>
        public void Cancel(DateTime utcNow)
        {
            if (Status != OrderStatus.PendingPayment)
                throw new InvalidStateTransitionException(nameof(Order), Status, OrderStatus.Cancelled);

            Status = OrderStatus.Cancelled;
            CancelledAtUtc = utcNow;
        }

        /// <summary>PendingPayment → Expired (hold lapsed before payment).</summary>
        public void Expire(DateTime utcNow)
        {
            if (Status != OrderStatus.PendingPayment)
                throw new InvalidStateTransitionException(nameof(Order), Status, OrderStatus.Expired);

            Status = OrderStatus.Expired;
            ExpiredAtUtc = utcNow;
        }
    }
}

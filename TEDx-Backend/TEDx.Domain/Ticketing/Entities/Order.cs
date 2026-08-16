using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.Abstractions;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Common.Exceptions;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Order : AuditableEntity, IHasRowVersion
    {
        public Guid Id { get; set; }
        public string? OrderReference { get; set; }
        public Guid AccountId { get; set; }
        public Guid EventId { get; set; }
        public Guid? PackageId { get; set; }
        public OrderUnitType UnitType { get; set; }
        public string? UnitNameSnapshot { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPriceSnapshot { get; set; }
        public decimal SubTotalSnapshot { get; set; }
        public decimal DiscountSnapshot { get; set; }
        public decimal TotalSnapshot { get; set; }
        public Guid? PromoCodeId { get; set; }
        public string? PromoCodeSnapshot { get; set; }
        public OrderStatus Status { get; private set; }
        public DateTime HoldExpiresAtUtc { get; set; }
        public DateTime PaidAtUtc { get; private set; }
        public DateTime CancelledAtUtc { get; private set; }
        public DateTime ExpiredAtUtc { get; private set; }
        public string? PaymobOrder { get; set; }
        public byte[] RowVersion { get; set; } = null!;
        // nav prop (intra-context only)
        public Event Event { get; set; } = null!;
        public List<Payment>? Payments { get; set; }
        public List<Ticket>? Tickets { get; set; }
        public List<RefundEntry>? RetryRefunds { get; set; }
        public List<PromoRedemption>? PromoRedemptions { get; set; }
        public Package? Package { get; set; }
        public PromoCode? PromoCode { get; set; }

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

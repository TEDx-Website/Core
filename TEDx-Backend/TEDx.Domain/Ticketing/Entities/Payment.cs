using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Payment : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string? PaymobOrderId { get; set; }
        public string? PaymobTransactionId { get; set; }
        public string? PaymentSessionId { get; set; }
        public string? IdempotencyKey { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? RawPayloadJson { get; set; }
        public Order Order { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Common.Entities;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Domain.Ticketing.Entities
{
    public class Payement : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; } // 1 order --> M PAyemt restrict
        public string? PaymobOrderId { get; set; }// 64
        public string? PaymobTransactionId { get; set; } // FUQ 64
        public string? PaymentSessionId { get; set; } // 128
        public string? IdempotencyKey { get; set; }// 64 FUQ
        public PaymentStatus PaymentStatus { get; set; } // NN DF
        public decimal Amount { get; set; } // 18,2
        public string? Currency { get; set; } // 5 nn df
        public string? RawPayloadJson { get; set; }// max
        public Order Order { get; set; } = null!;

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Ticketing.DTOs
{
    public sealed record EventOrderDto(
        BuyerDTO Buyer,
        string Status,
        int Quantity,
        MoneyDto Total,
        DateTime CreatedAtUtc,
        DateTime? HoldExpiresAtUtc
    );
}

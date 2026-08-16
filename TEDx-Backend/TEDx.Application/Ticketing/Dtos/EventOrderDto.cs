using TEDx.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Ticketing.Dtos
{
    public sealed record EventOrderDto(
        Guid Id,
        Guid BuyerId,
        string Status,
        int Quantity,
        MoneyDto Total,
        DateTime CreatedAtUtc,
        DateTime? HoldExpiresAtUtc
    );
}

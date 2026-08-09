using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Ticketing.DTOs
{
    public sealed record MoneyDto(
        decimal Amount,
        string Currency
    );
}

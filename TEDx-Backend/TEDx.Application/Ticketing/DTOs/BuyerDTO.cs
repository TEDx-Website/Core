using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Ticketing.DTOs
{
    public sealed record BuyerDTO(
        Guid Id,
        string FirstName,
        string LastName,
        string Email
    );
}



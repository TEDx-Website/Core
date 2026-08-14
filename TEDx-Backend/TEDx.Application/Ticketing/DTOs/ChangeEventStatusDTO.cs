using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Domain.Ticketing.Enums;

namespace TEDx.Application.Ticketing.DTOs
{
    public class ChangeEventStatusDTO
    {
        public EventStatus Status { get; set; }
        public byte[]? RowVersion { get; set; } 
    }
}

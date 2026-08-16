using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Common.Abstractions
{
    public interface IAuditable
    {
        DateTime CreatedAtUtc { get; set; }
        string? CreatedBy { get; set; }
        DateTime? UpdatedAtUtc { get; set; }
        string? UpdatedBy { get; set; }
        
    }
}

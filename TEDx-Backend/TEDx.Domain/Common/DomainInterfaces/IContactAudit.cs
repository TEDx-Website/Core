using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Common.DomainInterfaces
{
    public interface IContactAudit
    {
        DateTime CreatedAtUtc { get; set; }
        DateTime UpdatedAtUtc { get; set; }
        Guid UpdatedBy { get; set; }
    }
}

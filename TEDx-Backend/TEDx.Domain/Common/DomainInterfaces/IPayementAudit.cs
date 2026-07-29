using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Common.DomainInterfaces
{
    public interface IPayementAudit
    {
         DateTime CreatedAtUtc { get; set; } // NN
         DateTime UpdatedAtUtc { set; get; }
    }
}

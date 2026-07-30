using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Common.DomainInterfaces
{
    public interface IRefreshTokenAudit
    {
         DateTime CreatedAtUtc { get; set; }
    }
}

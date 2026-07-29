using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Common.DomainInterfaces
{
    public interface IConcurrent
    {
        byte[] RowVersion { get; set; }
    }
}

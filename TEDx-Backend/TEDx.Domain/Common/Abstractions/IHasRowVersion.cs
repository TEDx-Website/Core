using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Common.Abstractions
{
    public interface IHasRowVersion
    {
        byte[] RowVersion { get; set; }
    }
}

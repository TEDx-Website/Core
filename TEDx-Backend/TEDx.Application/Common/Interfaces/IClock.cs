using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Application.Common.Interfaces
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}

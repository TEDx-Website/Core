using System;
using System.Collections.Generic;
using System.Text;
using TEDx.Application.Common.Interfaces;

namespace TEDx.Infrastructure.Common
{
    public sealed class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TEDx.Domain.Common.Errors
{
    public readonly record struct Error(
        string Code,
        string Message,
        ErrorType Type
    );
}

using Serilog.Core;
using Serilog.Events;

namespace TEDx.Infrastructure.Logging;

internal sealed class SensitiveDataDestructuringPolicy : IDestructuringPolicy
{
    private static readonly HashSet<string> _maskedNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Password", "Secret", "Token", "Hash", "Qr", "QrSecret", "Pan", "CardNumber"
    };

    public bool TryDestructure(object value, ILogEventPropertyValueFactory factory, out LogEventPropertyValue result)
    {
        result = null!;
        return false;
    }

    internal static LogEventPropertyValue MaskIfSensitive(string name, LogEventPropertyValue value) =>
        IsSensitive(name) ? new ScalarValue("***") : value;

    internal static bool IsSensitive(string name) =>
        _maskedNames.Any(m => name.Contains(m, StringComparison.OrdinalIgnoreCase));
}

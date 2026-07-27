using Serilog.Core;
using Serilog.Events;

namespace TEDx.Infrastructure.Logging;

internal sealed class SensitivePropertyEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var sensitive = logEvent.Properties
            .Where(p => SensitiveDataDestructuringPolicy.IsSensitive(p.Key))
            .ToList();

        foreach (var (key, _) in sensitive)
            logEvent.AddOrUpdateProperty(new LogEventProperty(key, new ScalarValue("***")));
    }
}

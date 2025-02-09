using SubhadraSolutions.Utils.Diagnostics;

namespace SubhadraSolutions.Utils.Instrumentation.Instruments;

public class MethodInstrumentationConfig(bool instrumentMetrics, MethodLogOption logOption)
{
    public bool InstrumentMetrics { get; } = instrumentMetrics;
    public MethodLogOption LogOption { get; } = logOption;
}
using SubhadraSolutions.Utils.Telemetry.Metrics;

namespace SubhadraSolutions.Utils.Exposition.Telemetry.Metrics;

public class APIMetrics(string name) : RequestProcessingMetrics(name);
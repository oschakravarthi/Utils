using System;
using System.IO;
using DummyClassLibrary;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using SubhadraSolutions.Utils.ApplicationInsights;
using SubhadraSolutions.Utils.Compression.Wim;
using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.Instrumentation.Instruments;
using SubhadraSolutions.Utils.IO;
using SubhadraSolutions.Utils.Telemetry.Metrics;

namespace DummyConsoleApp;

internal class Program
{
    private static void TestWim()
    {
        var target = new DirectoryInfo(@"c:\temp\try");
        var result = WimHelper.ExtractAsync(@"c:\temp\wim.wim", target).Result;
    }

    private static void Main(string[] args)
    {
        FindReplaceHelper.ReplaceInDirectoriesAndFiles(@"C:\Personal\Github\SubhadraSolutions\PAA", "Astrology", "PAA", true, true, true, "*.*");
    }

    private static void TestInstrumentation()
    {
        MetricsTracker.Instance.Enabled = true;
        var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
        telemetryConfiguration.ConnectionString =
            "InstrumentationKey=81d00450-8252-49b3-a8a4-2df6c63c04f4;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/";
        var telemetryClient = new TelemetryClient(telemetryConfiguration);
        var metricsPublisher = new MetricsPublisher(telemetryClient);
        MethodInstrument.Logger = new DummyLogger();
        MethodInstrument.ConfigSelector = methodInfo =>
        {
            if (methodInfo.Name.StartsWith("Dummy"))
            {
                return new MethodInstrumentationConfig(true, MethodLogOption.LogAll);
            }

            return null;
        };

        Run();
    }

    private static void Run()
    {
        try
        {
            DummyClass.DummyMethod();
        }
        catch (Exception ex)
        {
        }
    }
}
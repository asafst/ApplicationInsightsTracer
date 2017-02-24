# Application Insights Tracer

This library is a custom tracer that wraps the [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) TelemetryClient class.

```C#
public static void Main(string[] args)
{
    AITracer aiTracer = AITracerFactory.CreateAITracer();
    aiTracer.TraceInformation("This is a informational test trace");
    aiTracer.TrackCustomEvent("Custom Event Name");
}
```

See here for more info about using of the [Application Insights Telemetry Client](https://github.com/Microsoft/ApplicationInsights-dotnet).

## Installation

You can obtain it [through Nuget](https://www.nuget.org/packages/ApplicationInsightsTracer/) with:
```
Install-Package ApplicationInsightsTracer
```

Or **clone** this repo and reference it.

## Initialization

You can create a new `AITracer` object via the `AITracerFactory`.
```C#
AITracer aiTracer = AITracerFactory.CreateAITracer();
```
This default mode uses the active [`TelemetryConfiguration`](https://github.com/Microsoft/ApplicationInsights-dotnet/blob/37cec526194b833f7cd676f25eafd985dd88d3fa/src/Core/Managed/Shared/Extensibility/TelemetryConfiguration.cs) and takes the Instrumentation Key from the `APPINSIGHTS_INSTRUMENTATIONKEY` app.settings value.

You can also create a new `TelemetryConfiguration` from the Applciation Insights SDK and pass it as a parameter:

```C#
TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();
config.InstrumentationKey = "MyIkey";
aiTracer = AITracerFactory.CreateAITracer(telemetryConfiguration: config);
```

## Usage

The `AITracer` object implements an interface for sending tracers and telemetry messages to Application Insights endpoint.

A full example with different telemetry options:

```C#
public static void ExampleWithDifferentTelemetryTypes()
{
    AITracer aiTracer = AITracerFactory.CreateAITracer();

    // Simple trace
    aiTracer.TraceInformation("Demo informational trace");

    try
    {
        aiTracer.TrackCustomEvent($"Demo Custom Event");
        throw new Exception("Demoing Failure");
    }
    catch (Exception e)
    {
        // Report the exception to see full exception details in the Application Insights portal (including full Stack Trace)
        aiTracer.ReportException(e);
    }

    // send a custom metric value 
    aiTracer.TrackCustomMetric("Demo metric", 42);

    // Remeber to flush the telemetry buffer before ending the process
    aiTracer.Flush();
}
```

You can see more examples in the DemoApplciation in the repository.


## Notes


## License

[MIT](LICENSE)

namespace DemoApplication
{
    using System;
    using System.Collections.Generic;
    using ApplicationInsightsTracer;
    using ApplicationInsightsTracer.OperationHandlers;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    class Program
    {
        static void Main(string[] args)
        {
            
        }

        public static void SimpleExample()
        {
            AITracer aiTracer = AITracerFactory.CreateAITracer();
            aiTracer.TraceInformation("This is a informational test trace");
            aiTracer.TrackCustomEvent("Custom Event Name");
        }

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

        public static void FullExampleWithOperation()
        {
            AITracer aiTracer = AITracerFactory.CreateAITracer();

            // Add a custom property to all traces
            aiTracer.AddCustomProperty("RunMode", "Demo");

            // Simple trace
            aiTracer.TraceInformation("First informational trace");

            int runIndex = 0;
            while (runIndex < 10)
            {
                // Create custom property for the operation that will be added to all traces in the operation
                var operationProperties = new Dictionary<string, string> { {"RunVersion", $"Run {runIndex}"} };

                // Create a operation to wrap all the current trigger telemetry under a single group.
                // It is disposable, so it will be dispatched by itself
                // See: https://docs.microsoft.com/en-us/azure/application-insights/app-insights-api-custom-events-metrics#operation-context
                using (aiTracer.StartOperation("Demo Operation", operationProperties))
                {
                    try
                    {
                        aiTracer.TrackCustomEvent($"Demo Custom Event");

                        // Throw exception sometimes
                        if (runIndex % 5 == 0)
                        {
                            throw new Exception("Demoing Failure");
                        }

                    }
                    catch (Exception e)
                    {
                        // Report the exception to see full exception details in the Application Insights portal (including full Stack Trace)
                        aiTracer.ReportException(e);

                        // Mark the operation as failure to see it in failed requests section
                        aiTracer.MarkOperationAsFailure();
                    }
                }

                runIndex++;
            }

            // send a custom metric value with the number of runs
            aiTracer.TrackCustomMetric("Total Number of Runs", runIndex);

            // Remeber to flush the telemetry buffer before ending the process
            aiTracer.Flush();
        }

        public static void InitializationExample()
        {
            // use the active telemetry confguration and take the instrumentation key from the App.config
            AITracer aiTracer = AITracerFactory.CreateAITracer();

            // Create a default telemetry configuration and set instrumentation key manually
            TelemetryConfiguration config = TelemetryConfiguration.CreateDefault();
            config.InstrumentationKey = "MyIkey";
            aiTracer = AITracerFactory.CreateAITracer(telemetryConfiguration: config);

            // Use a dependency operation handler instead of the default request one
            config = AITracerFactory.GetActiveTelemetryConfiguration();
            var operationHandler = new ApplicationInsightsOperationHandler<DependencyTelemetry>(new TelemetryClient(config));
            aiTracer = AITracerFactory.CreateAITracer(telemetryConfiguration: config, operationHandler: operationHandler);
        }
    }

}

namespace ApplicationInsightsTracer
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using ApplicationInsightsTracer.OperationHandlers;
    using ApplicationInsightsTracer.OtherTracers;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// A static factory class for create AI tracers
    /// </summary>
    public class AITracerFactory
    {

        private const string ApplicationInsightsInstrumentationKeyAppSettingsValue = "APPINSIGHTS_INSTRUMENTATIONKEY";

        public static IAITracer CreateAITracer(TelemetryConfiguration telemetryConfiguration = null,
            string sessionId = null,
            ITelemetryOperationHandler operationHandler = null)
        {
            if (telemetryConfiguration == null)
            {
                telemetryConfiguration = GetActiveTelemetryConfiguration();
            }

            var telemetryClient = new TelemetryClient(telemetryConfiguration);
            telemetryClient.Context.Session.Id = sessionId ?? Guid.NewGuid().ToString();

            // initialize reqeust operation holder
            operationHandler = operationHandler ?? new ApplicationInsightsRequestOperationHandler(telemetryClient);

            return new AITracer(telemetryClient, operationHandler);
        }

        public static IAITracer CreateAggregatedTracer(TelemetryConfiguration telemetryConfiguration = null,
            string sessionId = null,
            ITelemetryOperationHandler operationHandler = null,
            IReadOnlyCollection<ITracer> additionalTracers = null)
        {
            var singleAITracer = CreateAITracer(telemetryConfiguration, sessionId, operationHandler);

            return new AIAggregatedTracer(singleAITracer, additionalTracers);
        }

        /// <summary>
        /// Gets the active <see cref="TelemetryConfiguration"/>.
        /// If the instrumentation key is not given, it is taken from the app settings
        /// </summary>
        /// <param name="instrumentationKey">the application insights instrumentation key</param>
        /// <returns>the active <see cref="TelemetryConfiguration"/></returns>
        public static TelemetryConfiguration GetActiveTelemetryConfiguration(string instrumentationKey = null)
        {
            TelemetryConfiguration.Active.InstrumentationKey = GetInstrumentationKey(instrumentationKey); ;

            return TelemetryConfiguration.Active;
        }

        /// <summary>
        /// Creates a new default <see cref="TelemetryConfiguration"/>.
        /// If the instrumentation key is not given, it is taken from the app settings
        /// </summary>
        /// <param name="instrumentationKey">the application insights instrumentation key</param>
        /// <returns>the active <see cref="TelemetryConfiguration"/></returns>
        public static TelemetryConfiguration CreateDefaultTelemetryConfiguration(string instrumentationKey = null)
        {
            var config = TelemetryConfiguration.CreateDefault();
            config.InstrumentationKey = GetInstrumentationKey(instrumentationKey);

            return config;
        }

        private static string GetInstrumentationKey(string instrumentationKey = null)
        {
            return instrumentationKey ?? ConfigurationManager.AppSettings[ApplicationInsightsInstrumentationKeyAppSettingsValue];
        }
    }
}
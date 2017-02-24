namespace ApplicationInsightsTracer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using ApplicationInsightsTracer.OperationHandlers;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    /// Implementation of the <see cref="ITracer"/> interface that traces to AppInsights.
    /// </summary>
    public class AITracer : ITracer, ITelemetryOperationHandler
    {
        /// <summary>
        /// The internal Application Insights telemetry client
        /// </summary>
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// Custom properties that will be added to all events sent to Application Insights
        /// </summary>
        private readonly IDictionary<string, string> _customProperties;

        /// <summary>
        /// Custom properties that will be added to all events in the current operation
        /// </summary>
        private IDictionary<string, string> _customOperationProperties;

        /// <summary>
        /// An implementation of the <see cref="ITelemetryOperationHandler"/> 
        /// </summary>
        private readonly ITelemetryOperationHandler _operationHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AITracer"/> class with an operation handler
        /// </summary>
        /// <param name="telemetryClient">a <see cref="TelemetryClient"/></param>
        /// <param name="operationHandler">the operation handler to use</param>
        public AITracer(TelemetryClient telemetryClient, ITelemetryOperationHandler operationHandler)
        {
            _telemetryClient = telemetryClient;
            _operationHandler = operationHandler;
                
            _customProperties = new Dictionary<string, string>();
            _customOperationProperties = new Dictionary<string, string>();
        }

        #region Implementation of ITracer

        /// <summary>
        /// Trace <paramref name="message"/> as Information message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        public void TraceInformation(string message)
        {
            this.Trace(message, SeverityLevel.Information);
        }

        /// <summary>
        /// Trace <paramref name="message"/> as Error message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        public void TraceError(string message)
        {
            this.Trace(message, SeverityLevel.Error);
        }

        /// <summary>
        /// Trace <paramref name="message"/> as Verbose message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        public void TraceVerbose(string message)
        {
            this.Trace(message, SeverityLevel.Verbose);
        }

        /// <summary>
        /// Trace <paramref name="message"/> as Warning message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        public void TraceWarning(string message)
        {
            this.Trace(message, SeverityLevel.Warning);
        }
        
        /// <summary>
        /// Send a custom metric value.
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="value">The metric value</param>
        /// <param name="properties">Named string values you can use to classify and filter metrics</param>
        /// <param name="count">The aggregated metric count</param>
        /// <param name="max">The aggregated metric max value</param>
        /// <param name="min">The aggregated metric min name</param>
        /// <param name="timestamp">The timestamp of the aggregated metric</param>
        public void TrackCustomMetric(string name, double value, IDictionary<string, string> properties = null, int? count = null, double? max = null, double? min = null, DateTime? timestamp = null)
        {
            var metricTelemetry = new MetricTelemetry(name, value)
            {
                Count = count,
                Max = max,
                Min = min
            };

            // set custom timestamp if exist
            if (timestamp.HasValue)
            {
                metricTelemetry.Timestamp = timestamp.Value;
            }

            this.SetTelemetryProperties(metricTelemetry, properties);
            _telemetryClient.TrackMetric(metricTelemetry);
        }

        /// <summary>
        /// Send information about an event.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="properties">Named string values you can use to classify and filter events</param>
        /// <param name="metrics">Dictionary of application-defined metrics</param>
        public void TrackCustomEvent(string eventName, IDictionary<string, string> properties = null,IDictionary<string, double> metrics = null)
        {
            var eventTelemetry = new EventTelemetry(eventName);
            this.SetTelemetryProperties(eventTelemetry, properties);

            if (metrics != null)
            {
                foreach (KeyValuePair<string, double> metric in metrics)
                {
                    eventTelemetry.Metrics.Add(metric.Key, metric.Value);
                }
            }

            _telemetryClient.TrackEvent(eventTelemetry);
        }

        /// <summary>
        /// Reports a runtime exception.
        /// It uses exception and trace entities with same operation id.
        /// </summary>
        /// <param name="exception">The exception to report</param>
        public void ReportException(Exception exception)
        {
            // Trace exception
            this.TraceError(exception.ToString());

            ExceptionTelemetry exceptionTelemetry = new ExceptionTelemetry(exception);
            this.SetTelemetryProperties(exceptionTelemetry);
            _telemetryClient.TrackException(exceptionTelemetry);
        }

        /// <summary>
        /// Send information about a dependency handled by the application.
        /// </summary>
        /// <param name="dependencyTypeName">the dependency type name</param>
        /// <param name="target">the target of dependency call</param>
        /// <param name="dependencyName">The dependency name.</param>
        /// <param name="data">The data associated with the current dependency call</param>
        /// <param name="startTime">The dependency call start time</param>
        /// <param name="duration">The time taken by the application to handle the dependency.</param>
        /// <param name="resultCode">the dependency result code</param>
        /// <param name="success">a boolean value indicating if the dependency call was successful</param>
        public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success)
        {
            var dependencyTelemetry = new DependencyTelemetry(dependencyTypeName, target, dependencyName, data, startTime, duration, resultCode, success);
            this.SetTelemetryProperties(dependencyTelemetry);
            _telemetryClient.TrackDependency(dependencyTelemetry);
        }

        /// <summary>
        /// Add a custom property to all telemetry events.
        /// </summary>
        /// <param name="key">the property key</param>
        /// <param name="value">the property value</param>
        public void AddCustomProperty(string key, string value)
        {
            if (_customProperties.ContainsKey(key))
            {
                _customProperties[key] = value;
            }
            else
            {
                _customProperties.Add(key, value);
            }
        }

        /// <summary>
        /// Add custom properties to all telemetry events.
        /// </summary>
        /// <param name="properties">the dictionary of properties</param>
        public void AddCustomProperties(IDictionary<string, string> properties)
        {
            foreach (var property in properties)
            {
                this.AddCustomProperty(property.Key, property.Value);
            }
        }

        /// <summary>
        /// Flushes the telemetry channel
        /// </summary>
        public void Flush()
        {
            _telemetryClient.Flush();
            Thread.Sleep(1000); // sleeping for a second to make the telemetry get sent
        }

        #endregion

        #region ITelemetryOperationHandler implementation

        /// <summary>
        /// Starts a new operation.
        /// </summary>
        /// <param name="operationName">The name of the operation, to be used as OperationName in all telemtry items along the operation</param>
        /// <param name="properties">custom properties to be added to all telemetry in the operation</param>
        public IDisposable StartOperation(string operationName, IDictionary<string, string> properties = null)
        {
            // Create a shallow copy
            var mergedProperties = _customProperties.ToDictionary(e => e.Key, e => e.Value);

            if (properties != null)
            {
                // add the operation propeties to the merged properties
                properties.ToList().ForEach(kvp => mergedProperties[kvp.Key] = kvp.Value);
                _customOperationProperties = properties;
            }
            
            return _operationHandler.StartOperation(operationName, mergedProperties);
        }

        /// <summary>
        /// Mark operation as failure
        /// </summary>
        public void MarkOperationAsFailure()
        {
            _operationHandler.MarkOperationAsFailure();
        }

        /// <summary>
        /// Sends an operation summary telemetry.
        /// </summary>
        public void DispatchOperation()
        {
            _operationHandler.DispatchOperation();

            // reset of operation properties
            _customOperationProperties = new Dictionary<string, string>();
        }
        public void Dispose()
        {
            this.DispatchOperation();
            this.Flush();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Traces the specified message to the telemetry client
        /// </summary>
        /// <param name="message">The message to trace</param>
        /// <param name="severityLevel">The message's severity level</param>
        private void Trace(string message, SeverityLevel severityLevel)
        {
            var traceTelemetry = new TraceTelemetry(message, severityLevel);
            _telemetryClient.TrackTrace(traceTelemetry);
        }

        private void SetTelemetryProperties(ISupportProperties telemetry, IDictionary<string, string> properties = null)
        {
            // Add the custom properties
            foreach (KeyValuePair<string, string> customProperty in _customProperties)
            {
                telemetry.Properties[customProperty.Key] = customProperty.Value;
            }

            // Add the custom operation properties (they override the general custom properties)
            foreach (KeyValuePair<string, string> customProperty in _customOperationProperties)
            {
                telemetry.Properties[customProperty.Key] = customProperty.Value;
            }

            // And finally, add the user-supplied properties
            if (properties != null)
            {
                foreach (KeyValuePair<string, string> customProperty in properties)
                {
                    telemetry.Properties[customProperty.Key] = customProperty.Value;
                }
            }
        }

        #endregion

    }
}

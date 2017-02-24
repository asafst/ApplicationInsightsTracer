namespace ApplicationInsightsTracer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface providing tracing capabilities
    /// </summary>
    public interface ITracer
    {
        /// <summary>
        /// Trace <paramref name="message"/> as Information message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        void TraceInformation(string message);

        /// <summary>
        /// Trace <paramref name="message"/> as Error message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        void TraceError(string message);

        /// <summary>
        /// Trace <paramref name="message"/> as Verbose message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        void TraceVerbose(string message);

        /// <summary>
        /// Trace <paramref name="message"/> as Warning message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        void TraceWarning(string message);

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
        void TrackCustomMetric(string name, double value, IDictionary<string, string> properties = null,
            int? count = null, double? max = null, double? min = null, DateTime? timestamp = null);

        /// <summary>
        /// Send information about an event.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="properties">Named string values you can use to classify and filter events</param>
        /// <param name="metrics">Dictionary of application-defined metrics</param>
        void TrackCustomEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);

        /// <summary>
        /// Reports a runtime exception.
        /// It uses exception and trace entities with same operation id.
        /// </summary>
        /// <param name="exception">The exception to report</param>
        void ReportException(Exception exception);

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
        void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success);

        /// <summary>
        /// Add a custom property to all telemetry events.
        /// </summary>
        /// <param name="key">the property key</param>
        /// <param name="value">the property value</param>
        void AddCustomProperty(string key, string value);

        /// <summary>
        /// Add custom properties to all telemetry events.
        /// </summary>
        /// <param name="properties">the dictionary of properties</param>
        void AddCustomProperties(IDictionary<string, string> properties);

        /// <summary>
        /// Flushes the current telemetry items in the internal queues.
        /// </summary>
        void Flush();
    }
}

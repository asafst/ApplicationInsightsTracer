namespace ApplicationInsightsTracer.OtherTracers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An aggregated class of <see cref="ITracer"/>s
    /// </summary>
    public class AggregatedTracer : ITracer
    {
        private readonly List<ITracer> _tracers;

        public AggregatedTracer(List<ITracer> tracers)
        {
            _tracers = tracers;
        }

        public void TraceInformation(string message)
        {
            _tracers.ForEach(t => t.TraceInformation(message));
        }

        public void TraceError(string message)
        {
            _tracers.ForEach(t => t.TraceError(message));
        }

        public void TraceVerbose(string message)
        {
            _tracers.ForEach(t => t.TraceVerbose(message));
        }

        public void TraceWarning(string message)
        {
            _tracers.ForEach(t => t.TraceWarning(message));
        }

        public void TrackCustomMetric(string name, double value, IDictionary<string, string> properties = null, int? count = null, double? max = null,
            double? min = null, DateTime? timestamp = null)
        {
            _tracers.ForEach(t => t.TrackCustomMetric(name, value, properties, count, max, min, timestamp));
        }

        public void TrackCustomEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            _tracers.ForEach(t => t.TrackCustomEvent(eventName, properties, metrics));
        }

        public void ReportException(Exception exception)
        {
            _tracers.ForEach(t => t.ReportException(exception));
        }

        public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data,
            DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success)
        {
            _tracers.ForEach(t => t.TrackDependency(dependencyTypeName, target, dependencyName, data, startTime, duration, resultCode, success));
        }

        public void AddCustomProperty(string key, string value)
        {
            _tracers.ForEach(t => t.AddCustomProperty(key, value));
        }

        public void AddCustomProperties(IDictionary<string, string> properties)
        {
            _tracers.ForEach(t => t.AddCustomProperties(properties));
        }

        public void Flush()
        {
            _tracers.ForEach(t => t.Flush());
        }
    }
}
namespace ApplicationInsightsTracer.OtherTracers
{
    using System;
    using System.Collections.Generic;
    using ApplicationInsightsTracer.OperationHandlers;

    /// <summary>
    /// An aggregated class for a single AITracer and additional list of <see cref="ITracer"/> implementations
    /// </summary>
    public class AIAggregatedTracer : IAITracer
    {
        private readonly ITelemetryOperationHandler _aiTracerOperationHandler;
        private readonly List<ITracer> _additionalTracers;

        public AIAggregatedTracer(IAITracer aiTracer, IReadOnlyCollection<ITracer> additionalTracers)
        {
            _additionalTracers = new List<ITracer> {aiTracer};
            _additionalTracers.AddRange(additionalTracers);

            _aiTracerOperationHandler = aiTracer;
        }

        public void TraceInformation(string message)
        {
            _additionalTracers.ForEach(t => t.TraceInformation(message));
        }

        public void TraceError(string message)
        {
            _additionalTracers.ForEach(t => t.TraceError(message));
        }

        public void TraceVerbose(string message)
        {
            _additionalTracers.ForEach(t => t.TraceVerbose(message));
        }

        public void TraceWarning(string message)
        {
            _additionalTracers.ForEach(t => t.TraceWarning(message));
        }

        public void TrackCustomMetric(string name, double value, IDictionary<string, string> properties = null, int? count = null, double? max = null,
            double? min = null, DateTime? timestamp = null)
        {
            _additionalTracers.ForEach(t => t.TrackCustomMetric(name, value, properties, count, max, min, timestamp));
        }

        public void TrackCustomEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            _additionalTracers.ForEach(t => t.TrackCustomEvent(eventName, properties, metrics));
        }

        public void ReportException(Exception exception)
        {
            _additionalTracers.ForEach(t => t.ReportException(exception));
        }

        public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data,
            DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success)
        {
            _additionalTracers.ForEach(t => t.TrackDependency(dependencyTypeName, target, dependencyName, data, startTime, duration, resultCode, success));
        }

        public void AddCustomProperty(string key, string value)
        {
            _additionalTracers.ForEach(t => t.AddCustomProperty(key, value));
        }

        public void AddCustomProperties(IDictionary<string, string> properties)
        {
            _additionalTracers.ForEach(t => t.AddCustomProperties(properties));
        }

        public void Flush()
        {
            _additionalTracers.ForEach(t => t.Flush());
        }

        public IDisposable StartOperation(string operationName, IDictionary<string, string> properties = null)
        {
            return _aiTracerOperationHandler.StartOperation(operationName, properties);
        }

        public void MarkOperationAsFailure()
        {
            _aiTracerOperationHandler.MarkOperationAsFailure();
        }

        public void DispatchOperation()
        {
            _aiTracerOperationHandler.DispatchOperation();
        }

        public void Dispose()
        {
            _aiTracerOperationHandler.Dispose();
        }
    }
}
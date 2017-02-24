namespace ApplicationInsightsTracer.OtherTracers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Implementation of the <see cref="ITracer"/> interface that traces to a <see cref="TextWriter"/> logger.
    /// </summary>
    public class TextWriterTracer : ITracer
    {
        private readonly TextWriter _logger;

        /// <summary>
        /// Initialized a new instance of the <see cref="TextWriterTracer"/> class.
        /// </summary>
        /// <param name="logger">The logger to send traces to</param>
        public TextWriterTracer(TextWriter logger) 
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            // we keep a synchronized instance since logging can occur from multiple threads
            _logger = TextWriter.Synchronized(logger);
        }

        public void TraceInformation(string message)
        {
            _logger.WriteLine(message);
        }

        public void TraceError(string message)
        {
            _logger.WriteLine($"Error: {message}");
        }

        public void TraceVerbose(string message)
        {
            _logger.WriteLine($"Verbose: {message}");
        }

        public void TraceWarning(string message)
        {
            _logger.WriteLine($"Warning: {message}");
        }

        public void TrackCustomMetric(string name, double value, IDictionary<string, string> properties = null, int? count = null, double? max = null,
            double? min = null, DateTime? timestamp = null)
        {
            _logger.WriteLine($"Metric: name-{name}, value-{value}");
        }

        public void TrackCustomEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            _logger.WriteLine($"Event: name={eventName}");
        }

        public void ReportException(Exception exception)
        {
            _logger.WriteLine($"Exception: {exception}");
        }

        public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data,
            DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success)
        {
            _logger.WriteLine($"Dependency: name={dependencyName}, target={target}, data={data}, duration={duration}, success={success}");
        }

        public void AddCustomProperty(string key, string value)
        {
            // do nothing
        }

        public void AddCustomProperties(IDictionary<string, string> properties)
        {
            // do nothing
        }

        public void Flush()
        {
            _logger.Flush();
        }
    }
}

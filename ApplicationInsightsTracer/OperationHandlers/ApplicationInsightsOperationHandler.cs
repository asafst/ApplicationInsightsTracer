namespace ApplicationInsightsTracer.OperationHandlers
{
    using System;
    using System.Collections.Generic;
    using ApplicationInsightsTracer.Exceptions;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;

    public class ApplicationInsightsOperationHandler<T> : ITelemetryOperationHandler where T : OperationTelemetry, new()
    {
        /// <summary>
        /// The internal operation handler (is not null only when an operation starts using the StartOperation method)
        /// </summary>
        protected IOperationHolder<T> OperationHolder;

        /// <summary>
        /// Custom properties that will be added to the operation request telemetry
        /// </summary>
        private IDictionary<string, string> _operationCustomProperties;

        /// <summary>
        /// The internal Application Insights telemetry client
        /// </summary>
        private readonly TelemetryClient _telemetryClient;

        public ApplicationInsightsOperationHandler(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public IDisposable StartOperation(string operationName, IDictionary<string, string> properties = null)
        {
            if (OperationHolder != null)
            {
                throw new OperationAlreadyStartedException($"operation {operationName} can't start becaus {OperationHolder.Telemetry.Name} already started");
            }

            _operationCustomProperties = properties;

            OperationHolder = _telemetryClient.StartOperation<T>(operationName);
            
            return this;
        }

        public virtual void MarkOperationAsFailure()
        {
            this.EnsureOperationStarted("Can't mark operation as failed");
            OperationHolder.Telemetry.Success = false;
        }

        public void DispatchOperation()
        {
            this.EnsureOperationStarted("Can't dispatch operation");

            // set custom properties
            foreach (KeyValuePair<string, string> customProperty in _operationCustomProperties)
            {
                OperationHolder.Telemetry.Properties[customProperty.Key] = customProperty.Value;
            }

            this.SetTelemetryCustomMetrics();

            _telemetryClient.StopOperation(OperationHolder);

            OperationHolder = null;
        }

        public void Dispose()
        {
            if (OperationHolder != null)
            {
                this.DispatchOperation();
            }
        }

        private void EnsureOperationStarted(string message)
        {
            if (OperationHolder == null)
            {
                throw new OperationNotStartedException($"Operation not started yet. {message}");
            }
        }

        protected virtual void SetTelemetryCustomMetrics()
        {
            // As simple <see cref= "OperationTelemetry" /> can't hold metrics,
            // derived classes must implement this method to add these metrics to the operation telemetry
        }





    }
}
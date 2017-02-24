namespace ApplicationInsightsTracer.OperationHandlers
{
    using System;
    using System.Collections.Generic;

    public interface ITelemetryOperationHandler : IDisposable
    {
        /// <summary>
        /// Starts a new operation.
        /// </summary>
        /// <param name="operationName">The name of the operation, to be used as OperationName in all telemtry items along the operation</param>
        /// <param name="properties">custom properties to add to the operation summary telemetry</param>
        IDisposable StartOperation(string operationName, IDictionary<string, string> properties = null);

        /// <summary>
        /// Mark operation as failure
        /// </summary>
        void MarkOperationAsFailure();

        /// <summary>
        /// Sends an operation summary telemetry.
        /// </summary>
        void DispatchOperation();
    }
}
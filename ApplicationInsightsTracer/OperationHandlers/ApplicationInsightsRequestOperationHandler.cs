namespace ApplicationInsightsTracer.OperationHandlers
{
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    public class ApplicationInsightsRequestOperationHandler : ApplicationInsightsOperationHandler<RequestTelemetry>
    {
        /// <summary>
        /// Custom metrics that will be added to the operation request telemetry
        /// </summary>
        private readonly IDictionary<string, double> _operationCustomMetrics;

        public ApplicationInsightsRequestOperationHandler(TelemetryClient telemetryClient, IDictionary<string, double> metrics = null)
            : base(telemetryClient)
        {
            _operationCustomMetrics = metrics;
        }

        public override void MarkOperationAsFailure()
        {
            base.MarkOperationAsFailure();
            OperationHolder.Telemetry.ResponseCode = "500";
        }

        protected override void SetTelemetryCustomMetrics()
        {
            if (_operationCustomMetrics != null)
            {
                foreach (var customMeasurement in _operationCustomMetrics)
                {
                    OperationHolder.Telemetry.Metrics[customMeasurement.Key] = customMeasurement.Value;
                }
            }
        }
    }
}
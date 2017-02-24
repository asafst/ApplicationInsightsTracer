namespace ApplicationInsightsTracer
{
    using ApplicationInsightsTracer.OperationHandlers;
    public interface IAITracer : ITracer, ITelemetryOperationHandler
    {
        
    }
}
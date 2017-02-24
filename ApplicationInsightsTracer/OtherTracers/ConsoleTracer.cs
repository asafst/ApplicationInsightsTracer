namespace ApplicationInsightsTracer.OtherTracers
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Implementation of the <see cref="ITracer"/> interface that traces to a <see cref="Console"/> logger.
    /// </summary>
    public class ConsoleTracer : TextWriterTracer
    {
        public ConsoleTracer() : base(Console.Out)
        {
        }
    }
}
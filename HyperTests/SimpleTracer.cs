using System;
using System.Net.Http;
using System.Web.Http.Tracing;

namespace HyperTests
{
    public class SimpleTracer : ITraceWriter
    {
        public void Trace(HttpRequestMessage request, string category, TraceLevel level,
                          Action<TraceRecord> traceAction)
        {
            var rec = new TraceRecord(request, category, level);
            traceAction(rec);
            WriteTrace(rec);
        }

        protected void WriteTrace(TraceRecord rec)
        {
            var message = string.Format(
                "{0};{1};{2};{3}",
                rec.Kind,
                rec.Operator,
                rec.Operation,
                rec.Message);

            var category = string.Format(
                "[{0}] [{1}] [{2}] [{3}]", DateTimeOffset.Now.ToString("yyyy/MM/dd hh:mm:ss.fff"), rec.Level, rec.RequestId, rec.Category);

            System.Diagnostics.Trace.WriteLine(message, category);
        }
    }
}
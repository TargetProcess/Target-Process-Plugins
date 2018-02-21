using System.Collections.Generic;
using Tp.Core.Annotations;

namespace Tp.Core.Diagnostics.Event
{
    [PublicAPI("event goes to external events aggregator - logstash or something")]
    public class DiagnosticEvent
    {
        public IDictionary<string, object> Data { get; }
        public bool IsDetail { get; }
        public DiagnosticEventMetadata Metadata { get; }

        public DiagnosticEvent(DiagnosticEventMetadata metadata, IDictionary<string, object> data,
            bool isDetail = false)
        {
            Metadata = metadata;
            Data = data;
            IsDetail = isDetail;
        }
    }
}

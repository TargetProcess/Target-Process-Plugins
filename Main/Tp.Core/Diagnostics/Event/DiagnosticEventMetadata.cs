using System;
using Tp.Core.Annotations;

namespace Tp.Core.Diagnostics.Event
{
    [PublicAPI("event goes to external events aggregator - logstash or something")]
    public class DiagnosticEventMetadata
    {
        public DiagnosticEventMetadata(string accountName, int? userId, DateTime createDate, Version version)
        {
            AccountName = accountName;
            UserId = userId;
            CreateDate = createDate;
            Version = version;
            Host = Environment.MachineName;
        }
        public string AccountName { get; }
        public int? UserId { get; }
        public DateTime CreateDate { get; }
        public Version Version { get; }
        public string Host { get; }
    }
}

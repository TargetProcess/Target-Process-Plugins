using System;
using System.Collections.Generic;
using Tp.Core.Annotations;

namespace Tp.Core.Diagnostics.Event
{
	[PublicAPI("event goes to external events aggregator - logstash or something")]
	public class DiagnosticEvent
	{
		public string AccountName { get; }
		public int? UserId { get; }
		public DateTime CreateDate { get; }
		public Version Version { get; }
		public IDictionary<string,object> Data { get; }
		public DiagnosticEvent(string accountName, int? userId, DateTime createDate, Version version, IDictionary<string, object> data)
		{
			AccountName = accountName;
			UserId = userId;
			CreateDate = createDate;
			Data = data;
			Version = version;
		}
	}
}
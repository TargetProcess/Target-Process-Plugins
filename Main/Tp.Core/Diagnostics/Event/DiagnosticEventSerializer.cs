using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Tp.Core.Diagnostics.Event
{
	public class DiagnosticEventSerializer
	{
		private readonly JsonSerializer _serializer;
		public DiagnosticEventSerializer()
		{
			var settings = new JsonSerializerSettings
			{
				Formatting = Formatting.None,
				DateFormatString = "yyyy-MM-dd HH:mm:ss,FFF",
				NullValueHandling = NullValueHandling.Ignore
			};
			_serializer = JsonSerializer.Create(settings);
		}

		public string Serialize(DiagnosticEvent ev)
		{
			IDictionary<string, object> data = new Dictionary<string, object>
			{
				{ "account", ev.AccountName },
				{ "user", ev.UserId },
				{ "dateUtc", ev.CreateDate },
				{ "version", ev.Version.ToString()}
			};
			foreach (var x in ev.Data)
			{
				data.Add(x.Key, x.Value);
			}
			using (TextWriter writer = new StringWriter())
			{
				_serializer.Serialize(writer, data);
				return writer.ToString();
			}
		}
	}
}
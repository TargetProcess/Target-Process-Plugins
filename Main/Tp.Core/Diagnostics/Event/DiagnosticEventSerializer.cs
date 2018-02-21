using System;
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
                DateFormatString = "yyyy-MM-dd HH:mm:ss,fff",
                NullValueHandling = NullValueHandling.Ignore
            };
            _serializer = JsonSerializer.Create(settings);
        }

        public string Serialize(DiagnosticEvent ev)
        {
            IDictionary<string, object> data = new Dictionary<string, object>
            {
                { "account", ev.Metadata.AccountName },
                { "user", ev.Metadata.UserId },
                { "dateUtc", ev.Metadata.CreateDate },
                { "version", ev.Metadata.Version.ToString() },
                { "hostname", ev.Metadata.Host }
            };

            foreach (var x in ev.Data)
            {
                data[x.Key] = Prepare(x.Value);
            }
            using (TextWriter writer = new StringWriter())
            {
                _serializer.Serialize(writer, data);
                return writer.ToString();
            }
        }

        private object Prepare(object value)
        {
            if (value == null)
            {
                return null;
            }
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Single:
                    return Math.Round((float) value, 1);
                case TypeCode.Double:
                    return Math.Round((double) value, 1);
                case TypeCode.Decimal:
                    return Math.Round((decimal) value, 1);
                default:
                    return value;
            }
        }
    }
}

using System;
using Newtonsoft.Json;

namespace Tp.Core
{
	public class MaybeJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var maybe = (IMaybe)value;
			serializer.Serialize(writer, maybe.HasValue ? maybe.Value : null);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(IMaybe).IsAssignableFrom(objectType);
		}
	}
}
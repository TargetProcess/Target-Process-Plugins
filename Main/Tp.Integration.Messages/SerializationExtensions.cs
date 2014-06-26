using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;

namespace Tp.Integration.Messages
{
	public static class SerializationExtensions
	{
		public static string Serialize<T>(this T obj) where T : class
		{
			return obj.Serialize(new Type[] { });
		}

		public static string Serialize<T>(this T obj, IEnumerable<Type> knownTypes) where T : class
		{
			List<Type> knownTypesList = knownTypes.ToList();
			knownTypesList.Add(obj.GetType());
			var serializer = new DataContractJsonSerializer(typeof(T), knownTypesList);

			var ms = new MemoryStream();
			serializer.WriteObject(ms, obj);

			return Encoding.UTF8.GetString(ms.ToArray());
		}

		public static T Deserialize<T>(this string content) where T : class
		{
			return content.Deserialize<T>(new Type[] { });
		}

		public static object Deserialize(this string content, Type contentValueType, params Type[] knownTypes)
		{
			var serializer = new DataContractJsonSerializer(contentValueType, knownTypes);
			byte[] buffer = Encoding.UTF8.GetBytes(content);
			return
				serializer.ReadObject(JsonReaderWriterFactory.CreateJsonReader(buffer, 0, buffer.Length, Encoding.UTF8,
																			   XmlDictionaryReaderQuotas.Max, null));
		}

		public static T Deserialize<T>(this string content, params Type[] knownTypes)
		{
			return (T)content.Deserialize(typeof(T), knownTypes);
		}
	}
}
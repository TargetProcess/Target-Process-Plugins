// -----------------------------------------------------------------------------------
// Use it as you please, but keep this header.
// Author : Marcus Deecke, 2006
// Web    : www.yaowi.com
// Email  : code@yaowi.com
// -----------------------------------------------------------------------------------
using System.IO;
using System.Xml;
using NServiceBus;
using NServiceBus.Serialization;

namespace Tp.Integration.Messages.ServiceBus.Serialization
{
	public class AdvancedXmlSerializer : IMessageSerializer
	{
		public void Serialize(IMessage[] messages, Stream stream)
		{
			using (var serializer = new XmlSerializer())
			{
				var serialized = serializer.Serialize(messages);
				using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { CheckCharacters = false }))
				{
					serialized.WriteTo(writer);
				}
			}
		}

		public IMessage[] Deserialize(Stream stream)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(stream);

			var deserializer = new XmlDeserializer();
			return (IMessage[]) deserializer.Deserialize(xmlDocument);
		}
	}
}
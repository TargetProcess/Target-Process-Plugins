// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Xml;
using NServiceBus;
using NServiceBus.Serialization;
using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Integration.Messages.ServiceBus.Serialization
{
	public class AdvancedXmlSerializer : IMessageSerializer
	{
		public void Serialize(IMessage[] messages, Stream stream)
		{
			using (var serializer = new XmlSerializer())
			{
				var serialized = serializer.Serialize(messages);
				using (var writer = XmlWriter.Create(stream, new XmlWriterSettings {CheckCharacters = false}))
				{
					serialized.WriteTo(writer);
				}
			}
		}

		public IMessage[] Deserialize(Stream stream)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(stream);

			try
			{
				return Deserialize(xmlDocument);
			}
			catch (Exception)
			{
				var patchedText = SerializationPatcher.Apply(xmlDocument.InnerXml);
				var patchedXmlDocument = new XmlDocument();
				patchedXmlDocument.LoadXml(patchedText);
				return Deserialize(patchedXmlDocument);
			}
		}

		private static IMessage[] Deserialize(XmlDocument xmlDocument)
		{
			var deserializer = new XmlDeserializer();
			return (IMessage[]) deserializer.Deserialize(xmlDocument);
		}
	}
}
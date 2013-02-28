// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tp.Core;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Storage.Persisters.Serialization
{
	public class JsonBlobSerializer : IBlobSerializer
	{
		public const string VERSION = "2";

		public object Deserialize(XDocument stateData, string keyType)
		{
			if (stateData == null)
			{
				return null;
			}

			using (var reader = stateData.CreateReader())
			{
				reader.ReadStartElement();
				var typeValue = reader.ReadElementString();

				// Ensure the Type was Specified)
				if (typeValue == null)
					throw new ArgumentNullException("Unable to Read Xml Data for Abstract Type '" + keyType +
					                                "' because no 'type' attribute was specified in the XML.");

				ValidateVersion(reader);

				var type = Type.GetType((new TypeNameWithoutVersion(typeValue)).Value);

				// Check the Type is Found.
				if (type == null)
					throw new InvalidCastException("Unable to Read Xml Data for Abstract Type '" + keyType +
					                               "' because the type specified in the XML was not found.");

				return reader.ReadElementString().Deserialize(type);
			}
		}

		public XDocument Serialize(object value)
		{
			var formatter = new XmlSerializer(typeof (string));
			var doc = new XDocument();
			using (var writer = doc.CreateWriter())
			{
				writer.WriteStartElement("Value");
				writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
				writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");

				AddStringElement(writer, "Type", (new TypeNameWithoutVersion(value.GetType())).Value);
				AddStringElement(writer, "Version", VERSION);

				formatter.Serialize(writer, value.Serialize());

				writer.WriteEndElement();
			}
			return doc;
		}

		private static void ValidateVersion(XmlReader reader)
		{
			var version = reader.ReadElementString();

			if (version != VERSION)
				throw new ApplicationException("Version of serialize data isn't correct.", new SerializationException());
		}

		private static void AddStringElement(XmlWriter writer, string name, string value)
		{
			writer.WriteStartElement(name);
			writer.WriteString(value);
			writer.WriteEndElement();
		}
	}
}
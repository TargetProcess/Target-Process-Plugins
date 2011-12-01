// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tp.Core;

namespace Tp.Integration.Plugin.Common.Storage.Persisters.Serialization
{
	//Don't change this file. It is old serializer to support previous version of serialization.
	public class XmlBlobSerializer : IBlobSerializer
	{
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

				// Ensure the Type was Specified
				if (typeValue == null)
					throw new ArgumentNullException("Unable to Read Xml Data for Abstract Type '" + keyType +
					                                "' because no 'type' attribute was specified in the XML.");

				var type = Type.GetType(((TypeNameWithoutVersion) typeValue).Value);

				// Check the Type is Found.
				if (type == null)
					throw new InvalidCastException("Unable to Read Xml Data for Abstract Type '" + keyType +
					                               "' because the type specified in the XML was not found.");

				return new XmlSerializer(type).Deserialize(reader.ReadSubtree());
			}
		}

		public XDocument Serialize(object value)
		{
			var formatter = new XmlSerializer(value.GetType());
			var doc = new XDocument();
			using (var writer = doc.CreateWriter())
			{
				writer.WriteStartElement("Value");
				writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
				writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");

				writer.WriteStartElement("Type");
				writer.WriteString(((TypeNameWithoutVersion) value.GetType()).Value);
				writer.WriteEndElement();

				formatter.Serialize(writer, value);

				writer.WriteEndElement();
			}
			return doc;
		}
	}
}
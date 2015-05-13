// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace Tp.Components
{
	/// <summary>
	/// Serialization helper class.
	/// </summary>
	public sealed class Serializer
	{
		/// <summary>
		/// Deserializes object of the specified Type from XML document 
		/// read from the specified text reader.
		/// </summary>
		/// <typeparam name = "T">Type of the deserialized object.</typeparam>
		/// <param name = "textReader">Text reader from which to read XML document from which to deserialize object.</param>
		/// <returns>Object deserialized from XML document read from the text reader.</returns>
		public static T Deserialize<T>(TextReader textReader)
		{
			return (T) Deserialize(typeof (T), textReader);
		}

		/// <summary>
		/// Deserializes object of the specified Type from the specified XML document.
		/// </summary>
		/// <typeparam name = "T">Type of the deserialized object.</typeparam>
		/// <param name = "xml">XML document from which to deserialize object.</param>
		/// <returns>Object deserialized from the XML document.</returns>
		public static T Deserialize<T>(string xml)
		{
			return (T) Deserialize(typeof (T), xml);
		}

		/// <summary>
		/// Serializes the specified object of specified type into XML document 
		/// written to the specified text writer.
		/// </summary>
		/// <typeparam name = "T">Type of the object being serialized as XML document.</typeparam>
		/// <param name = "item">Object being serialized as XML document.</param>
		/// <param name = "textWriter">Text writer into which XML document to be written.</param>
		public static void Serialize<T>(T item, TextWriter textWriter)
		{
			Serialize(typeof (T), item, textWriter);
		}

		/// <summary>
		/// Serializes the specified object of specified type into XML document.
		/// </summary>
		/// <typeparam name = "T">Type of the object being serialized as XML document.</typeparam>
		/// <param name = "item">Object being serialized as XML document.</param>
		/// <returns>XML document containing serialized object representation.</returns>
		public static string Serialize<T>(T item)
		{
			return Serialize(typeof (T), item);
		}

		/// <summary>
		/// Deserializes object of the specified Type from the specified XML document.
		/// </summary>
		/// <param name = "type">Type of the deserialized object.</param>
		/// <param name = "xml">XML document from which to deserialize object.</param>
		/// <returns>Object deserialized from the XML document.</returns>
		public static object Deserialize(Type type, string xml)
		{
			return Deserialize(type, new StringReader(xml));
		}

		/// <summary>
		/// Deserializes object of the specified Type from XML document 
		/// read from the specified text reader.
		/// </summary>
		/// <param name = "type">Type of the deserialized object.</param>
		/// <param name = "textReader">Text reader from which to read XML document from which to deserialize object.</param>
		/// <returns>Object deserialized from XML document read from the text reader.</returns>
		public static object Deserialize(Type type, TextReader textReader)
		{
			bool isHashTable = type.FullName == typeof (Hashtable).FullName;
			Type typeToSerialize = isHashTable ? typeof (ArrayList) : type;

			var xmlSerializer = new XmlSerializer(typeToSerialize);
			object objectToReturn = xmlSerializer.Deserialize(textReader);

			if (isHashTable)
			{
				var hashtable = new Hashtable();
				var keys = (ArrayList) ((ArrayList) objectToReturn)[0];
				var values = (ArrayList) ((ArrayList) objectToReturn)[1];

				for (int i = 0; i < keys.Count; i++)
				{
					hashtable.Add(keys[i], values[i]);
				}

				return hashtable;
			}

			return objectToReturn;
		}

		/// <summary>
		/// Serializes the specified object of specified type into XML document.
		/// </summary>
		/// <param name = "type">Type of the object being serialized as XML document.</param>
		/// <param name = "item">Object being serialized as XML document.</param>
		/// <returns>XML document containing serialized object representation.</returns>
		public static string Serialize(Type type, object item)
		{
			TextWriter writer = new StringWriter();
			Serialize(type, item, writer);
			return writer.ToString();
		}

		/// <summary>
		/// Serializes the specified object of specified type into XML document 
		/// written to the specified text writer.
		/// </summary>
		/// <param name = "type">Type of the object being serialized as XML document.</param>
		/// <param name = "item">Object being serialized as XML document.</param>
		/// <param name = "textWriter">Text writer into which XML document to be written.</param>
		public static void Serialize(Type type, object item, TextWriter textWriter)
		{
			bool isHashTable = type.FullName == typeof (Hashtable).FullName;
			Type typeToSerialize = isHashTable ? typeof (ArrayList) : type;

			var xmlSerializer = new XmlSerializer(typeToSerialize);

			if (isHashTable)
			{
				var arrayList = new ArrayList();
				var hashtable = (Hashtable) item;
				arrayList.Add(new ArrayList(hashtable.Keys));
				arrayList.Add(new ArrayList(hashtable.Values));
				xmlSerializer.Serialize(textWriter, arrayList);
			}
			else
			{
				xmlSerializer.Serialize(textWriter, item);
			}
		}
	}
}
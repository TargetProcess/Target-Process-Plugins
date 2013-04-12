// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Integration.Plugin.Common.Storage.Persisters.Serialization
{
	public class BlobSerializer
	{
		private static readonly List<IBlobSerializer> Serializers = new List<IBlobSerializer>
		                                             	{
		                                             		new JsonBlobSerializer(),
		                                             		new XmlBlobSerializer()
		                                             	};

		public static object Deserialize(XDocument stateData, string keyType)
		{
			try
			{
				return DeserializeInternal(stateData, keyType);
			}
			catch (Exception)
			{
				var reader = stateData.CreateReader();
				reader.MoveToContent();
				var patchedText = SerializationPatcher.Apply(reader.ReadOuterXml(), new RemoveBackingFieldPatch(keyType));

				return DeserializeInternal(XDocument.Parse(patchedText), keyType);
			}
		}

		private static object DeserializeInternal(XDocument stateData, string keyType)
		{
			foreach (IBlobSerializer serializer in Serializers)
			{
				try
				{
					return serializer.Deserialize(stateData, keyType);
				}
				catch (Exception)
				{
					continue;
				}
			}
			throw new ApplicationException(string.Format("Can't deserialize plugin storage data of type '{0}'", keyType));
		}

		public static XDocument Serialize(object value)
		{
			return Serializers[0].Serialize(value);
		}
	}
}
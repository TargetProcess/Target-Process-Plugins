// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var readOuterXml = reader.ReadOuterXml();
                var serializationPatcher = new SerializationPatcher(new[] { new RemoveBackingFieldPatch(keyType) });
                if (serializationPatcher.ShouldApply(readOuterXml))
                {
                    var patchedText = serializationPatcher.Apply(readOuterXml);
                    return DeserializeInternal(XDocument.Parse(patchedText), keyType);
                }

                throw;
            }
        }

        private static object DeserializeInternal(XDocument stateData, string keyType)
        {
            List<Exception> errors = new List<Exception>();
            foreach (IBlobSerializer serializer in Serializers)
            {
                try
                {
                    return serializer.Deserialize(stateData, keyType);
                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }
            var errorDetails = errors.Aggregate(new StringBuilder(), (acc, e) => acc.AppendLine(e.ToString()).AppendLine());
            throw new ApplicationException($"Can't deserialize plugin storage data of type '{keyType}'. Details: {errorDetails}");
        }

        public static XDocument Serialize(object value)
        {
            return Serializers[0].Serialize(value);
        }
    }
}

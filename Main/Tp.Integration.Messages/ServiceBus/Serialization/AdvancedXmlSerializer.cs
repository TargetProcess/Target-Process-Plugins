//
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.IO;
using System.Linq;
using System.Xml;
using log4net;
using NServiceBus;
using NServiceBus.Serialization;
using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Integration.Messages.ServiceBus.Serialization
{
    public class AdvancedXmlSerializer : IMessageSerializer
    {
        private readonly ILog _logger = LogManager.GetLogger("Deserialiaztion");

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
            var result = Deserialize(xmlDocument);

            var notFoundTypesCount = result.Item2;
            if (notFoundTypesCount == 0)
            {
                return result.Item1;
            }

            var serializationPatcher = new SerializationPatcher(Array.Empty<IPatch>());
            if (!serializationPatcher.ShouldApply(xmlDocument.InnerXml))
            {
                return result.Item1;
            }

            return ApplyPatchesAndDeserialize(serializationPatcher, xmlDocument);
        }

        private IMessage[] ApplyPatchesAndDeserialize(SerializationPatcher serializationPatcher, XmlDocument xmlDocument)
        {
            var patchedText = serializationPatcher.Apply(xmlDocument.InnerXml);
            var patchedXmlDocument = new XmlDocument();
            patchedXmlDocument.LoadXml(patchedText);
            return Deserialize(patchedXmlDocument).Item1;
        }

        private Tuple<IMessage[], int> Deserialize(XmlDocument xmlDocument)
        {
            var notFoundTypesCount = 0;

            var deserializer = new XmlDeserializer(e =>
            {
                notFoundTypesCount++;
                _logger.Warn(e);
            });

            var messages = (IMessage[]) deserializer.Deserialize(xmlDocument);
            return Tuple.Create(messages.WhereNotNull().ToArray(), notFoundTypesCount);
        }
    }
}

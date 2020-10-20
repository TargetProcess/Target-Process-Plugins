using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tp.Integration.Messages.Entities;
using Tp.Model.Common.Entities.CustomField;
using Tp.Model.Common.Interfaces;

namespace Tp.Model.Common
{
    public static class CustomFieldConfigSerializer
    {
        public static readonly ICustomFieldConfigSerializer Instance = new CustomFieldConfigJsonSerializer();

        // Prevents JSON serialization attacks as JSON may come from external source.
        // See https://stackoverflow.com/questions/39565954/typenamehandling-caution-in-newtonsoft-json
        private class WhiteListSerializationBinder : ISerializationBinder
        {
            // Thread-safe, so can be singleton. See implementation for reference.
            private static readonly ISerializationBinder _defaultBinder = new DefaultSerializationBinder();
            // Expect small length (< 10), so hashset is slower here.
            private readonly Type[] _typesWhiteList;

            public WhiteListSerializationBinder(Type[] typesWhiteList)
            {
                _typesWhiteList = typesWhiteList;
            }

            public Type BindToType(string assemblyName, string typeName)
            {
                var type = _defaultBinder.BindToType(assemblyName, typeName);
                return _typesWhiteList.Contains(type) ? type : null;
            }

            public void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                _defaultBinder.BindToName(serializedType, out assemblyName, out typeName);
            }
        }

        private class CustomFieldConfigJsonSerializer : ICustomFieldConfigSerializer
        {
            private static readonly Type[] _knownConfigTypes = FormatInfo.KnownTypes.Concat(typeof(CustomFieldConfig)).ToArray();
            private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = new WhiteListSerializationBinder(_knownConfigTypes)
            };

            public string Serialize(CustomFieldConfig config) => JsonConvert.SerializeObject(config, Formatting.None, _jsonSettings);

            public CustomFieldConfig Deserialize(string stringConfig) =>
                JsonConvert.DeserializeObject<CustomFieldConfig>(stringConfig, _jsonSettings);
        }
    }
}

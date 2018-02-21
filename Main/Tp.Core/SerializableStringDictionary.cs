using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace Tp.Core
{
    [XmlRoot("stringDictionary")]
    public class XmlSerializableStringDictionary
    {
        #region Construction and Initialization

        public XmlSerializableStringDictionary(Dictionary<string, string> original)
        {
            _original = original;
        }

        /// <summary>
        /// Default constructor so deserialization works
        /// </summary>
        public XmlSerializableStringDictionary()
        {
        }

        #endregion

        #region The Proxy List

        /// <summary>
        /// Holds the keys and values
        /// </summary>
        public class KeyAndValue
        {
            [XmlAttribute("key")]
            public string Key { get; set; }

            [XmlAttribute("value")]
            public string Value { get; set; }
        }

        // This field will store the deserialized list
        private Collection<KeyAndValue> _list;
        private Dictionary<string, string> _original;

        /// <remarks>
        /// XmlElementAttribute is used to prevent extra nesting level. It's
        /// not necessary.
        /// </remarks>
        [XmlElement("add")]
        public Collection<KeyAndValue> KeysAndValues
        {
            get
            {
                if (_list == null)
                {
                    _list = new Collection<KeyAndValue>();
                }

                // On deserialization, Original will be null, just return what we have
                if (_original == null)
                {
                    return _list;
                }

                // If Original was present, add each of its elements to the list
                _list.Clear();
                foreach (var pair in _original)
                {
                    _list.Add(new KeyAndValue { Key = pair.Key, Value = pair.Value });
                }

                return _list;
            }
        }

        #endregion

        /// <summary>
        /// Convenience method to return a dictionary from this proxy instance
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ToDictionary()
        {
            return KeysAndValues.ToDictionary(key => key.Key, value => value.Value);
        }
    }
}

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Mercurial.XmlSerializationTypes
{
    /// <summary>
    /// This class encapsulates a &lt;tag&gt;...&lt;/tag&gt; node in the log output.
    /// </summary>
    [XmlType("tag")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class LogEntryTagNode
    {
        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        private string _Name = string.Empty;

        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        [XmlText]
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = (value ?? string.Empty).Trim();
            }
        }
    }
}
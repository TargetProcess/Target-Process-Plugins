using System.ComponentModel;
using System.Xml.Serialization;

namespace Mercurial.XmlSerializationTypes
{
    /// <summary>
    /// This class encapsulates a &lt;copy...&gt; node in the log output.
    /// </summary>
    [XmlType("copy")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class LogEntryCopyNode
    {
        /// <summary>
        /// This is the backing field for the <see cref="Destination"/> property.
        /// </summary>
        private string _Destination = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="Source"/> property.
        /// </summary>
        private string _Source = string.Empty;

        /// <summary>
        /// Gets or sets the source of the copy.
        /// </summary>
        [XmlAttribute("source")]
        public string Source
        {
            get
            {
                return _Source;
            }

            set
            {
                _Source = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets the destination of the copy.
        /// </summary>
        [XmlText]
        public string Destination
        {
            get
            {
                return _Destination;
            }

            set
            {
                _Destination = (value ?? string.Empty).Trim();
            }
        }
    }
}
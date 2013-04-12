using System.ComponentModel;
using System.Xml.Serialization;

namespace Mercurial.XmlSerializationTypes
{
    /// <summary>
    /// This class encapsulates a &lt;parent...&gt; node in the log output.
    /// </summary>
    [XmlType("parent")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class LogEntryParentNode
    {
        /// <summary>
        /// Gets or sets the revision of the parent log entry.
        /// </summary>
        [XmlAttribute("revision")]
        public int Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the hash of the parent log entry.
        /// </summary>
        [XmlAttribute("node")]
        public string Hash
        {
            get;
            set;
        }
    }
}
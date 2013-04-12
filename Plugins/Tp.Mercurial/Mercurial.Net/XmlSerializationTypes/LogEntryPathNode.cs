using System.ComponentModel;
using System.Xml.Serialization;

namespace Mercurial.XmlSerializationTypes
{
    /// <summary>
    /// This class encapsulates a &lt;path...&gt; node in the log output.
    /// </summary>
    [XmlType("path")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class LogEntryPathNode
    {
        /// <summary>
        /// Gets or sets the type of action performed on the path.
        /// </summary>
        [XmlAttribute("action")]
        public string Action
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path that was involved.
        /// </summary>
        [XmlText]
        public string Path
        {
            get;
            set;
        }
    }
}
using System.ComponentModel;
using System.Xml.Serialization;

namespace Mercurial.XmlSerializationTypes
{
    /// <summary>
    /// This class encapsulates a &lt;author...&gt; node in the log output.
    /// </summary>
    [XmlType("author")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class LogEntryAuthorNode
    {
        /// <summary>
        /// Gets or sets the name of the author of the log entry.
        /// </summary>
        [XmlText]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the email address of the author of the log entry.
        /// </summary>
        [XmlAttribute("email")]
        public string Email
        {
            get;
            set;
        }
    }
}
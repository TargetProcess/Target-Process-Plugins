using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Mercurial.XmlSerializationTypes
{
    /// <summary>
    /// This class encapsulates a &lt;logentry...&gt; node in the log output.
    /// </summary>
    [XmlType("logentry")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class LogEntryNode
    {
        /// <summary>
        /// This is the backing field for the <see cref="Copies"/> property.
        /// </summary>
        private readonly List<LogEntryCopyNode> _Copies = new List<LogEntryCopyNode>();

        /// <summary>
        /// This is the backing field for the <see cref="Parents"/> property.
        /// </summary>
        private readonly List<LogEntryParentNode> _Parents = new List<LogEntryParentNode>();

        /// <summary>
        /// This is the backing field for the <see cref="PathActions"/> property.
        /// </summary>
        private readonly List<LogEntryPathNode> _PathActions = new List<LogEntryPathNode>();

        /// <summary>
        /// This is the backing field for the <see cref="Tags"/> property.
        /// </summary>
        private readonly List<LogEntryTagNode> _Tags = new List<LogEntryTagNode>();

        /// <summary>
        /// Gets or sets the local revision number of this log entry.
        /// </summary>
        [XmlAttribute("revision")]
        public int Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the hash of this log entry.
        /// </summary>
        [XmlAttribute("node")]
        public string Hash
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the tags of this log entry.
        /// </summary>
        [XmlElement("tag")]
        public Collection<LogEntryTagNode> Tags
        {
            get
            {
                return new Collection<LogEntryTagNode>(_Tags);
            }
        }

        /// <summary>
        /// Gets or sets the commit message of this log entry.
        /// </summary>
        [XmlElement("msg")]
        public string CommitMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the timestamp of this log entry.
        /// </summary>
        [XmlElement("date")]
        public DateTime Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the author of this log entry.
        /// </summary>
        [XmlElement("author")]
        public LogEntryAuthorNode Author
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of parents of this log entry.
        /// </summary>
        [XmlElement("parent")]
        public Collection<LogEntryParentNode> Parents
        {
            get
            {
                return new Collection<LogEntryParentNode>(_Parents);
            }
        }

        /// <summary>
        /// Gets or sets the named branch this log entry is on.
        /// </summary>
        [XmlElement("branch")]
        public string Branch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the individual path actions of this log entry.
        /// </summary>
        [XmlArray("paths")]
        public Collection<LogEntryPathNode> PathActions
        {
            get
            {
                return new Collection<LogEntryPathNode>(_PathActions);
            }
        }

        /// <summary>
        /// Gets the copies, Add actions that are really copies of existing sources.
        /// </summary>
        [XmlArray("copies")]
        public Collection<LogEntryCopyNode> Copies
        {
            get
            {
                return new Collection<LogEntryCopyNode>(_Copies);
            }
        }
    }
}
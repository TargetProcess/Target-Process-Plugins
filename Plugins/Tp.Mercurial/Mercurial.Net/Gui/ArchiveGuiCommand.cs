using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "archive" command (<see href="http://tortoisehg.bitbucket.org/manual/2.0/archive.html#from-command-line"/>):
    /// Create an unversioned archive of a repository revision.
    /// </summary>
    public sealed class ArchiveGuiCommand : GuiCommandBase<ArchiveGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveGuiCommand"/> class.
        /// </summary>
        public ArchiveGuiCommand()
            : base("archive")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the <see cref="Revision"/> to archive.
        /// </summary>
        [NullableArgument]
        [DefaultValue(null)]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this <see cref="ArchiveGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ArchiveGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ArchiveGuiCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }
    }
}
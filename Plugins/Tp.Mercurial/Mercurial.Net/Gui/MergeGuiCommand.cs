using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "merge" command:
    /// Open the merge tool.
    /// </summary>
    public sealed class MergeGuiCommand : GuiCommandBase<MergeGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergeGuiCommand"/> class.
        /// </summary>
        public MergeGuiCommand()
            : base("merge")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> identifying the revision to merge with.
        /// </summary>
        [NullableArgument(NonNullOption = "--rev"), DefaultValue(null)]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this <see cref="MergeGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="MergeGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public MergeGuiCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }
    }
}
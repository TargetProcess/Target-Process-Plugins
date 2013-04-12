using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "backout" command:
    /// Backout tool.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class BackoutGuiCommand : GuiCommandBase<BackoutGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackoutGuiCommand"/> class.
        /// </summary>
        public BackoutGuiCommand()
            : base("backout")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to merge with old dirstate parent after backout.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--clean")]
        [DefaultValue(false)]
        public bool Merge
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Merge"/> property to the specified value and
        /// returns this <see cref="BackoutGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BackoutGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BackoutGuiCommand WithMerge(bool value = true)
        {
            Merge = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> to back out.
        /// </summary>
        [NullableArgument(NonNullOption = "--rev")]
        [DefaultValue(null)]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this <see cref="BackoutGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BackoutGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BackoutGuiCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> of the parent to choose when backing out merge.
        /// </summary>
        [NullableArgument(NonNullOption = "--parent")]
        [DefaultValue(null)]
        public RevSpec Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Parent"/> property to the specified value and
        /// returns this <see cref="BackoutGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Parent"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="BackoutGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BackoutGuiCommand WithParent(RevSpec value)
        {
            Parent = value;
            return this;
        }
    }
}
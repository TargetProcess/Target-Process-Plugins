using System;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "tag" command:
    /// Open the tag tool.
    /// </summary>
    public sealed class TagGuiCommand : GuiCommandBase<TagGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Action"/> property.
        /// </summary>
        private TagAction _Action = TagAction.Add;

        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        private string _Name = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="CommitMessage"/> property.
        /// </summary>
        private string _CommitMessage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagGuiCommand"/> class.
        /// </summary>
        public TagGuiCommand()
            : base("tag")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add or remove the tag.
        /// Default is <see cref="TagAction.Add"/>.
        /// </summary>
        [DefaultValue(TagAction.Add)]
        [EnumArgument(TagAction.Remove, "--remove")]
        public TagAction Action
        {
            get
            {
                return _Action;
            }

            set
            {
                _Action = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="Action"/> property to the specified value and
        /// returns this <see cref="TagGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Action"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="TagGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public TagGuiCommand WithAction(TagAction value)
        {
            Action = value;
            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to replace an existing tag, in effect moving the tag to
        /// a different changeset.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--force")]
        [DefaultValue(false)]
        public bool ReplaceExisting
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="ReplaceExisting"/> property to the specified value and
        /// returns this <see cref="TagGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ReplaceExisting"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="TagGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public TagGuiCommand WithReplaceExisting(bool value = true)
        {
            ReplaceExisting = value;
            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add or remove a local tag. If <c>false</c>, a changeset
        /// will be committed that edits the .hgtags file accordingly.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--local")]
        [DefaultValue(false)]
        public bool IsLocal
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="IsLocal"/> property to the specified value and
        /// returns this <see cref="TagGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="IsLocal"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="TagGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public TagGuiCommand WithIsLocal(bool value = true)
        {
            IsLocal = value;
            return this;
        }

        /// <summary>
        /// Gets or sets which revision to tag, or <c>null</c> for the parent of the
        /// working folder.
        /// Default is <c>null</c>.
        /// </summary>
        [DefaultValue(null)]
        [NullableArgument(NonNullOption = "--rev")]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this <see cref="TagGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="TagGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public TagGuiCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the name of the tag to add or remove.
        /// </summary>
        [NullableArgument]
        [DefaultValue("")]
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

        /// <summary>
        /// Sets the <see cref="Name"/> property to the specified value and
        /// returns this <see cref="TagGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Name"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="TagGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public TagGuiCommand WithName(string value)
        {
            Name = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the commit message to use, or leave blank to use a generated one.
        /// Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument(NonNullOption = "--message")]
        [DefaultValue("")]
        public string CommitMessage
        {
            get
            {
                return _CommitMessage;
            }

            set
            {
                _CommitMessage = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="CommitMessage"/> property to the specified value and
        /// returns this <see cref="TagGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="CommitMessage"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="TagGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public TagGuiCommand WithCommitMessage(string value)
        {
            CommitMessage = value;
            return this;
        }
    }
}
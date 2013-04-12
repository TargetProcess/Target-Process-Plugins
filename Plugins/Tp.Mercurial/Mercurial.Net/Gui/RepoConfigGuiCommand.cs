using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "repoconfig" command:
    /// Show the repository configuration editor.
    /// </summary>
    public sealed class RepoConfigGuiCommand : GuiCommandBase<RepoConfigGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="FieldToFocus"/> property.
        /// </summary>
        private string _FieldToFocus = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepoConfigGuiCommand"/> class.
        /// </summary>
        public RepoConfigGuiCommand()
            : base("repoconfig")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the field to give initial focus.
        /// </summary>
        [NullableArgument(NonNullOption = "--focus")]
        [DefaultValue("")]
        public string FieldToFocus
        {
            get
            {
                return _FieldToFocus;
            }

            set
            {
                _FieldToFocus = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the value of the <see cref="FieldToFocus"/> property to the
        /// specified value and returns this <see cref="RepoConfigGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to set the <see cref="FieldToFocus"/> property to.
        /// </param>
        /// <returns>
        /// This <see cref="RepoConfigGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RepoConfigGuiCommand WithFieldToFocus(string value)
        {
            FieldToFocus = value;
            return this;
        }
    }
}
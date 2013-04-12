using System;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "mpatch" command:
    /// Attempt to resolve conflicts in a .rej file.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyGTK"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyGTK)]
    public sealed class MPatchGuiCommand : GuiCommandBase<MPatchGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="File"/> property.
        /// </summary>
        private string _File = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MPatchGuiCommand"/> class.
        /// </summary>
        public MPatchGuiCommand()
            : base("mpatch")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the filename to attempt to resolve conflicts in.
        /// Default value is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument]
        [DefaultValue("")]
        public string File
        {
            get
            {
                return _File;
            }

            set
            {
                _File = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="File"/> property to the specified value and
        /// returns this <see cref="MPatchGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="File"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="MPatchGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public MPatchGuiCommand WithFile(string value)
        {
            File = value;
            return this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">File must be set before executing a MPatchGuiCommand</exception>
        public override void Validate()
        {
            base.Validate();

            if (StringEx.IsNullOrWhiteSpace(File))
                throw new InvalidOperationException("File must be set before executing a MPatchGuiCommand");
        }
    }
}
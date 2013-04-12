using System;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "annotate" command:
    /// Annotate dialog.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class AnnotateGuiCommand : BrowserGuiCommandBase<AnnotateGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="File"/> property.
        /// </summary>
        private string _File = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotateGuiCommand"/> class.
        /// </summary>
        public AnnotateGuiCommand()
            : base("annotate")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the file to annotate.
        /// Default value is <c>string.Empty</c>.
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
        /// returns this <see cref="AnnotateGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="File"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="AnnotateGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public AnnotateGuiCommand WithFile(string value)
        {
            File = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the line number to open the annotation to.
        /// Default value is <c>0</c>.
        /// </summary>
        [NullableArgument(NonNullOption = "--line")]
        [DefaultValue(0)]
        public int? LineNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="LineNumber"/> property to the specified value and
        /// returns this <see cref="AnnotateGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="LineNumber"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="AnnotateGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public AnnotateGuiCommand WithLineNumber(int value)
        {
            LineNumber = value;
            return this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para>The <see cref="AnnotateGuiCommand"/> requires a <see cref="File"/> to annotate</para>
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (StringEx.IsNullOrWhiteSpace(File))
                throw new InvalidOperationException("The AnnotateGuiCommand requires a File to annotate");
        }
    }
}
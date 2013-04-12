using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "vdiff" command:
    /// Launch the visual diff tool.
    /// </summary>
    public sealed class DiffGuiCommand : FilesBasedGuiCommandBase<DiffGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="BundleFile"/> property.
        /// </summary>
        private string _BundleFile = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffGuiCommand"/> class.
        /// </summary>
        public DiffGuiCommand()
            : base("vdiff")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the bundle file to preview.
        /// </summary>
        [NullableArgument(NonNullOption = "--bundle")]
        [DefaultValue("")]
        public string BundleFile
        {
            get
            {
                return _BundleFile;
            }

            set
            {
                _BundleFile = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="BundleFile"/> property to the specified value and
        /// returns this <see cref="DiffGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="BundleFile"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="DiffGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffGuiCommand WithBundleFile(string value)
        {
            BundleFile = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the specific changeset to view in the diff tool.
        /// </summary>
        [NullableArgument(NonNullOption = "--change")]
        [DefaultValue(null)]
        public RevSpec Changeset
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Changeset"/> property to the specified value and
        /// returns this <see cref="DiffGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Changeset"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="DiffGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffGuiCommand WithChangeset(RevSpec value)
        {
            Changeset = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the revision <see cref="RevSpec"/> that identifies the revision or the
        /// revision range to view a diff of.
        /// Default value is <c>null</c>.
        /// </summary>
        [NullableArgument(NonNullOption = "--rev")]
        [DefaultValue(null)]
        public RevSpec Revisions
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Revisions"/> property to the specified value and
        /// returns this <see cref="DiffGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revisions"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="DiffGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffGuiCommand WithRevisions(RevSpec value)
        {
            Revisions = value;
            return this;
        }
    }
}
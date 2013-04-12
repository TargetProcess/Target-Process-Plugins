using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "update" command:
    /// Update/checkout changeset to working directory.
    /// </summary>
    public sealed class UpdateGuiCommand : GuiCommandBase<UpdateGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Clean"/> property.
        /// </summary>
        private bool _Clean;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateGuiCommand"/> class.
        /// </summary>
        public UpdateGuiCommand()
            : base("update")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> of the revision to update to.
        /// Default value is <c>null</c>.
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
        /// returns this <see cref="UpdateGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="UpdateGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public UpdateGuiCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to do a clean update, discarding uncommitted changes in the process (no backup.)
        /// Default is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// This property is only available for the <see cref="GuiClientType.PyQT"/> client type.
        /// </remarks>
        [BooleanArgument(TrueOption = "--clean")]
        [DefaultValue(false)]
        public bool Clean
        {
            get
            {
                return _Clean;
            }

            set
            {
                if (_Clean == value)
                    return;
                
                EnsurePropertyAvailability("Clean", GuiClientType.PyQT);
                _Clean = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="Clean"/> property to the specified value and
        /// returns this <see cref="UpdateGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Clean"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="UpdateGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public UpdateGuiCommand WithClean(bool value = true)
        {
            Clean = value;
            return this;
        }
    }
}
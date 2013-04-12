using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "strip" command:
    /// Strip changesets from the repository.
    /// </summary>
    public sealed class StripGuiCommand : GuiCommandBase<StripGuiCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="ForceDiscardUncommittedChanges"/> property.
        /// </summary>
        private bool _ForceDiscardUncommittedChanges;

        /// <summary>
        /// This is the backing field for the <see cref="CreateBackup"/> property.
        /// </summary>
        private bool _CreateBackup = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="StripGuiCommand"/> class.
        /// </summary>
        public StripGuiCommand()
            : base("strip")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force discarding of uncommitted changes (Warning: there is no backup of the discarded changes;
        /// the <see cref="CreateBackup"/> property decides whether to make a backup of the stripped changesets.)
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--force")]
        [DefaultValue(false)]
        public bool ForceDiscardUncommittedChanges
        {
            get
            {
                return _ForceDiscardUncommittedChanges;
            }

            set
            {
                if (_ForceDiscardUncommittedChanges == value)
                    return;
                
                EnsurePropertyAvailability("ForceDiscardUncommittedChanges", GuiClientType.PyQT);
                _ForceDiscardUncommittedChanges = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="ForceDiscardUncommittedChanges"/> property to the specified value and
        /// returns this <see cref="StripGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ForceDiscardUncommittedChanges"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="StripGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public StripGuiCommand WithForceDiscardUncommittedChanges(bool value = true)
        {
            ForceDiscardUncommittedChanges = value;
            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to create a backup of the stripped revisions.
        /// Default value is <c>true</c>.
        /// </summary>
        [BooleanArgument(FalseOption = "--nobackup")]
        [DefaultValue(true)]
        public bool CreateBackup
        {
            get
            {
                return _CreateBackup;
            }

            set
            {
                if (_CreateBackup == value)
                    return;
                
                EnsurePropertyAvailability("CreateBackup", GuiClientType.PyQT);
                _CreateBackup = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="CreateBackup"/> property to the specified value and
        /// returns this <see cref="StripGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="CreateBackup"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="StripGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public StripGuiCommand WithCreateBackup(bool value)
        {
            CreateBackup = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> of the revision to strip.
        /// Default value is <c>null</c>.
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
        /// returns this <see cref="StripGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="StripGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public StripGuiCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }
    }
}
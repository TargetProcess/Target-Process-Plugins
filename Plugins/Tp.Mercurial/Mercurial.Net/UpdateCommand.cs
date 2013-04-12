using System;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg update" command (<see href="http://www.selenic.com/mercurial/hg.1.html#update"/>):
    /// update working directory (or switch revisions.)
    /// </summary>
    public sealed class UpdateCommand : MercurialCommandBase<UpdateCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="MergeTool"/> property.
        /// </summary>
        private string _MergeTool = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="AcrossBranches"/> property.
        /// </summary>
        private bool _AcrossBranches;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommand"/> class.
        /// </summary>
        public UpdateCommand()
            : base("update")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> to update the working copy to.
        /// </summary>
        [NullableArgument]
        [DefaultValue(null)]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to do a clean update, discarding uncommitted changes in the process (no backup.)
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--clean")]
        [DefaultValue(false)]
        public bool Clean
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to update across branches if there are no uncommitted changes.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--check")]
        [DefaultValue(false)]
        public bool AcrossBranches
        {
            get
            {
                return _AcrossBranches;
            }

            set
            {
                RequiresVersion(new Version(1, 7), "AcrossBranches property of the UpdateCommand class");
                _AcrossBranches = value;
            }
        }

        /// <summary>
        /// Gets or sets the merge tool to use for rebase.
        /// Default value is <see cref="string.Empty"/>, which means the default merge tool is to
        /// be used.
        /// </summary>
        [DefaultValue("")]
        [NullableArgument(NonNullOption = "--tool")]
        public string MergeTool
        {
            get
            {
                return _MergeTool;
            }

            set
            {
                _MergeTool = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="MergeTool"/> property to the specified value and
        /// returns this <see cref="UpdateCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="MergeTool"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="UpdateCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public UpdateCommand WithMergeTool(string value)
        {
            MergeTool = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this <see cref="UpdateCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="UpdateCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public UpdateCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Clean"/> property to the specified value and
        /// returns this <see cref="UpdateCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Clean"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="UpdateCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public UpdateCommand WithClean(bool value = true)
        {
            Clean = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="AcrossBranches"/> property to the specified value and
        /// returns this <see cref="UpdateCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="AcrossBranches"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="UpdateCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public UpdateCommand WithAcrossBranches(bool value = true)
        {
            AcrossBranches = value;
            return this;
        }
    }
}
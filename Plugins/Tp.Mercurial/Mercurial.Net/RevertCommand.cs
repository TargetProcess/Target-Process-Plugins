using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg revert" command (<see href="http://www.selenic.com/mercurial/hg.1.html#revert"/>):
    /// restore individual files or directories to an earlier state.
    /// </summary>
    public sealed class RevertCommand : IncludeExcludeCommandBase<RevertCommand>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Names"/> property.
        /// </summary>
        private readonly List<string> _Names = new List<string>();

        /// <summary>
        /// This is the backing field for the <see cref="SaveBackup"/> property.
        /// </summary>
        private bool _SaveBackup = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="RevertCommand"/> class.
        /// </summary>
        public RevertCommand()
            : base("revert")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the collection of names (of files or directories) to revert.
        /// </summary>
        public Collection<string> Names
        {
            get
            {
                return new Collection<string>(_Names);
            }
        }

        /// <summary>
        /// Adds the value to the <see cref="Names"/> collection property and
        /// returns this <see cref="RevertCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Names"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="RevertCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="value"/> is <c>null</c> or empty.</para>
        /// </exception>
        public RevertCommand WithName(string value)
        {
            if (StringEx.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            Names.Add(value);
            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to store backup files (.orig) when reverting changes.
        /// Default value is <c>true</c>.
        /// </summary>
        [BooleanArgument(FalseOption = "--no-backup")]
        [DefaultValue(true)]
        public bool SaveBackup
        {
            get
            {
                return _SaveBackup;
            }

            set
            {
                _SaveBackup = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="SaveBackup"/> property to the specified value and
        /// returns this <see cref="RevertCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="SaveBackup"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="RevertCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RevertCommand WithSaveBackup(bool value)
        {
            SaveBackup = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> value to revert the files to;
        /// or <c>null</c> to not use date-specification.
        /// Default value is <c>null</c>.
        /// </summary>
        [DateTimeArgument(NonNullOption = "--date")]
        [DefaultValue(null)]
        public DateTime? RevertToDate
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="RevertToDate"/> property to the specified value and
        /// returns this <see cref="RevertCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="SaveBackup"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="RevertCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RevertCommand WithRevertToDate(DateTime? value)
        {
            RevertToDate = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> to revert to.
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
        /// returns this <see cref="RevertCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="RevertCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RevertCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        public override IEnumerable<string> Arguments
        {
            get
            {
                return base.Arguments.Concat(Names);
            }
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para><see cref="Names"/> is empty.</para>
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (_Names.Count == 0)
                throw new InvalidOperationException("Must specify at least one name of a directory or file to revert before executing the RevertCommand");
        }

        /// <summary>
        /// This method should throw the appropriate exception depending on the contents of
        /// the <paramref name="exitCode"/> and <paramref name="standardErrorOutput"/>
        /// parameters, or simply return if the execution is considered successful.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the command line client.
        /// </param>
        /// <param name="standardErrorOutput">
        /// The standard error output from executing the command client.
        /// </param>
        /// <exception cref="MercurialExecutionException">
        /// <para><paramref name="exitCode"/> is not <c>0</c>.</para>
        /// </exception>
        protected override void ThrowOnUnsuccessfulExecution(int exitCode, string standardErrorOutput)
        {
            base.ThrowOnUnsuccessfulExecution(exitCode, standardErrorOutput);

            if (!StringEx.IsNullOrWhiteSpace(standardErrorOutput))
                throw new MercurialExecutionException(standardErrorOutput);
        }
    }
}
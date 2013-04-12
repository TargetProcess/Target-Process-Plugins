using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg diff" command (<see href="http://www.selenic.com/mercurial/hg.1.html#diff"/>):
    /// diff repository (or selected files).
    /// </summary>
    public sealed class DiffCommand : IncludeExcludeCommandBase<DiffCommand>, IMercurialCommand<string>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Names"/> property.
        /// </summary>
        private readonly ListFile _Names = new ListFile();

        /// <summary>
        /// This is the backing field for the <see cref="Revisions"/> property.
        /// </summary>
        private readonly List<RevSpec> _Revisions = new List<RevSpec>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffCommand"/> class.
        /// </summary>
        public DiffCommand()
            : base("diff")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the collection <see cref="RevSpec"/> that identifies the revision(s) or the
        /// revision range(s) to view a diff of.
        /// </summary>
        [RepeatableArgument(Option = "--rev")]
        public Collection<RevSpec> Revisions
        {
            get
            {
                return new Collection<RevSpec>(_Revisions);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> identifying a revision, where all changes
        /// introduced by that changeset will be returned.
        /// </summary>
        [NullableArgument(NonNullOption = "--change")]
        [DefaultValue(null)]
        public RevSpec ChangeIntroducedByRevision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to omit dates from diff headers.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--nodates")]
        [DefaultValue(false)]
        public bool OmitDatesFromHeaders
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the specified value to the <see cref="Revisions"/> property and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Revisions"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithRevisions(RevSpec value)
        {
            Revisions.Add(value);
            return this;
        }

        /// <summary>
        /// Gets the collection of names (of files or directories) to revert.
        /// </summary>
        public Collection<string> Names
        {
            get
            {
                return _Names.Collection;
            }
        }

        /// <summary>
        /// Adds the values to the <see cref="Names"/> collection property and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="values">
        /// The values to add to the <see cref="Names"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithNames(params string[] values)
        {
            if (values != null)
                Names.AddRange(values);

            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to follow renames and copies when limiting the log.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--git")]
        [DefaultValue(false)]
        public bool UseGitDiffFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to treat all files as text.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--text")]
        [DefaultValue(false)]
        public bool TreatAllFilesAsText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show which function each change is in.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--show-function")]
        [DefaultValue(false)]
        public bool ShowFunctions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to produce a reversal diff, one that would undo
        /// the change introduced by the changeset(s).
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--reverse")]
        [DefaultValue(false)]
        public bool ProduceReverseDiff
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to recurse into subrepositories.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--subrepos")]
        [DefaultValue(false)]
        public bool RecurseSubRepositories
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the types of changes to ignore.
        /// Default value is <see cref="DiffIgnores.None"/>.
        /// </summary>
        [DefaultValue(DiffIgnores.None)]
        public DiffIgnores Ignore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of lines of context to show for each diff. Use 0 to leave at default.
        /// Default value is <c>0</c>.
        /// </summary>
        [DefaultValue(0)]
        public int ContextLineCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to output diffstat-style of summary of changes instead
        /// of the full diff.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--stat")]
        [DefaultValue(false)]
        public bool SummaryOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="RecurseSubRepositories"/> property to the specified value and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="RecurseSubRepositories"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithRecurseSubRepositories(bool value)
        {
            RecurseSubRepositories = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="UseGitDiffFormat"/> property to the specified value and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="UseGitDiffFormat"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        [Obsolete("Use WithUseGitDiffFormat instead")]
        public DiffCommand WithGitDiffFormat(bool value = true)
        {
            UseGitDiffFormat = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ChangeIntroducedByRevision"/> property to the specified value and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ChangeIntroducedByRevision"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithChangeIntroducedByRevision(RevSpec value)
        {
            ChangeIntroducedByRevision = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Ignore"/> property to the specified value and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Ignore"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithIgnore(DiffIgnores value)
        {
            Ignore = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="UseGitDiffFormat"/> property to the specified value and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="UseGitDiffFormat"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithUseGitDiffFormat(bool value = true)
        {
            UseGitDiffFormat = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="SummaryOnly"/> property to the specified value and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="SummaryOnly"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithSummaryOnly(bool value = true)
        {
            SummaryOnly = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ContextLineCount"/> property to the specified value and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ContextLineCount"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithContextLineCount(int value)
        {
            ContextLineCount = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ProduceReverseDiff"/> property to the specified value and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ProduceReverseDiff"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithProduceReverseDiff(bool value = true)
        {
            ProduceReverseDiff = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ShowFunctions"/> property to the specified value and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ShowFunctions"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithShowFunctions(bool value = true)
        {
            ShowFunctions = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OmitDatesFromHeaders"/> property to the specified value and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="OmitDatesFromHeaders"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithOmitDatesFromHeaders(bool value = true)
        {
            OmitDatesFromHeaders = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="TreatAllFilesAsText"/> property to the specified value and
        /// returns this <see cref="DiffCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="TreatAllFilesAsText"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="DiffCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public DiffCommand WithTreatAllFilesAsText(bool value = true)
        {
            TreatAllFilesAsText = value;
            return this;
        }

        /// <summary>
        /// This method should parse and store the appropriate execution result output
        /// according to the type of data the command line client would return for
        /// the command.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the command line client.
        /// </param>
        /// <param name="standardOutput">
        /// The standard output from executing the command line client.
        /// </param>
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to call
        /// the base method at all.
        /// </remarks>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            base.ParseStandardOutputForResults(exitCode, standardOutput);

            if (exitCode == 0)
                Result = standardOutput;
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public string Result
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        public override IEnumerable<string> Arguments
        {
            get
            {
                List<string> arguments = base.Arguments.Concat(_Names.GetArguments()).ToList();

                if ((Ignore & DiffIgnores.WhiteSpace) != 0)
                    arguments.Add("--ignore-all-space");
                if ((Ignore & DiffIgnores.ChangedWhiteSpace) != 0)
                    arguments.Add("--ignore-space-change");
                if ((Ignore & DiffIgnores.BlankLines) != 0)
                    arguments.Add("--ignore-blank-lines");

                if (ContextLineCount > 0)
                {
                    arguments.Add("--unified");
                    arguments.Add(ContextLineCount.ToString(CultureInfo.InvariantCulture));
                }

                return arguments;
            }
        }

        /// <summary>
        /// Override this method to implement code that will execute after command
        /// line execution.
        /// </summary>
        protected override void Cleanup()
        {
            base.Cleanup();
            _Names.Cleanup();
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to call
        /// the base method at all.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="ContextLineCount"/> property of the <see cref="DiffCommand"/> has to be 0 or higher.
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (ContextLineCount < 0)
                throw new InvalidOperationException("The ContextLineCount property of the DiffCommand has to be 0 or higher");
        }
    }
}
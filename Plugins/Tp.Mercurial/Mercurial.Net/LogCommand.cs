using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg log" command (<see href="http://www.selenic.com/mercurial/hg.1.html#log"/>):
    /// show revision history of entire repository or files.
    /// </summary>
    public sealed class LogCommand : IncludeExcludeCommandBase<LogCommand>, IMercurialCommand<IEnumerable<Changeset>>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Users"/> property.
        /// </summary>
        private readonly List<string> _Users = new List<string>();

        /// <summary>
        /// This is the backing field for the <see cref="Keywords"/> property.
        /// </summary>
        private readonly List<string> _Keywords = new List<string>();

        /// <summary>
        /// This is the backing field for the <see cref="Revisions"/> property.
        /// </summary>
        private readonly List<RevSpec> _Revisions = new List<RevSpec>();

        /// <summary>
        /// This is the backing field for the <see cref="Path"/> property.
        /// </summary>
        private string _Path = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="IncludeHiddenChangesets"/> property.
        /// </summary>
        private bool _IncludeHiddenChangesets;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCommand"/> class.
        /// </summary>
        public LogCommand()
            : base("log")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the date to show the log for, or <c>null</c> if no filtering on date should be done.
        /// Default is <c>null</c>.
        /// </summary>
        [DateTimeArgument(NonNullOption = "--date", Format = "yyyy-MM-dd")]
        [DefaultValue(null)]
        public DateTime? Date
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of case-insensitive keywords to search the log for.
        /// Default is empty which indicates no searching will be done.
        /// </summary>
        [RepeatableArgument(Option = "--keyword")]
        public Collection<string> Keywords
        {
            get
            {
                return new Collection<string>(_Keywords);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to follow renames and copies when limiting the log.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--follow")]
        [DefaultValue(false)]
        public bool FollowRenamesAndMoves
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to only follow the first parent of merge changesets.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--follow-first")]
        [DefaultValue(false)]
        public bool OnlyFollowFirstParent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include path actions (which files were modified, and the
        /// type of modification) or not. Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--verbose")]
        [DefaultValue(false)]
        public bool IncludePathActions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include hidden changesets.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--hidden")]
        [DefaultValue(false)]
        public bool IncludeHiddenChangesets
        {
            get
            {
                return _IncludeHiddenChangesets;
            }

            set
            {
                RequiresVersion(new Version(1, 9), "The IncludeHiddenChangesets property of the LogCommand");
                _IncludeHiddenChangesets = value;
            }
        }

        /// <summary>
        /// Gets or sets the path to produce the log for, or <see cref="string.Empty"/> to produce
        /// the log for the repository. Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument]
        [DefaultValue("")]
        public string Path
        {
            get
            {
                return _Path;
            }

            set
            {
                _Path = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets the collection of users to produce the log for, or an empty collection to produce
        /// the log for the repository. Default is an empty collection.
        /// </summary>
        [RepeatableArgument(Option = "--user")]
        public Collection<string> Users
        {
            get
            {
                return new Collection<string>(_Users);
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="Revisions"/> to process/include.
        /// </summary>
        [RepeatableArgument(Option = "--rev")]
        public Collection<RevSpec> Revisions
        {
            get
            {
                return new Collection<RevSpec>(_Revisions);
            }
        }

        #region IMercurialCommand<IEnumerable<Changeset>> Members

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        /// <value></value>
        public override IEnumerable<string> Arguments
        {
            get
            {
                var arguments = new[]
                {
                    "--style=XML", "-v"
                };
                return arguments.Concat(base.Arguments).ToArray();
            }
        }

        /// <summary>
        /// Gets the result of executing the command as a collection of <see cref="Changeset"/> objects.
        /// </summary>
        public IEnumerable<Changeset> Result
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Sets the <see cref="Date"/> property to the specified value and
        /// returns this <see cref="LogCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Date"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommand WithDate(DateTime value)
        {
            Date = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="FollowRenamesAndMoves"/> property to the specified value and
        /// returns this <see cref="LogCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="FollowRenamesAndMoves"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommand WithFollowRenamesAndMoves(bool value = true)
        {
            FollowRenamesAndMoves = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OnlyFollowFirstParent"/> property to the specified value and
        /// returns this <see cref="LogCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="OnlyFollowFirstParent"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommand WithOnlyFollowFirstParent(bool value = true)
        {
            OnlyFollowFirstParent = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IncludePathActions"/> property to the specified value and
        /// returns this <see cref="LogCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="IncludePathActions"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommand WithIncludePathActions(bool value = true)
        {
            IncludePathActions = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IncludeHiddenChangesets"/> property to the specified value and
        /// returns this <see cref="LogCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="IncludeHiddenChangesets"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommand WithIncludeHiddenChangesets(bool value = true)
        {
            IncludeHiddenChangesets = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Path"/> property to the specified value and
        /// returns this <see cref="LogCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Path"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommand WithPath(string value)
        {
            Path = value;
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Revisions"/> collection property and
        /// returns this <see cref="LogCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Revisions"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommand WithRevision(RevSpec value)
        {
            Revisions.Add(value);
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Keywords"/> collection property and
        /// returns this <see cref="LogCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Keywords"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommand WithKeyword(string value)
        {
            Keywords.Add(value);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Users"/> property to the specified value and
        /// returns this <see cref="LogCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Users"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="LogCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public LogCommand WithUser(string value)
        {
            Users.Add(value);
            return this;
        }

        /// <summary>
        /// Parses the standard output for results.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        /// <param name="standardOutput">The standard output.</param>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            base.ParseStandardOutputForResults(exitCode, standardOutput);
            Result = ChangesetXmlParser.LazyParse(standardOutput);
        }
    }
}
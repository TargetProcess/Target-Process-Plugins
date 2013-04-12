using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg commit" command (<see href="http://www.selenic.com/mercurial/hg.1.html#commit"/>):
    /// commit the specified files or all outstanding changes.
    /// </summary>
    public sealed class CommitCommand : IncludeExcludeCommandBase<CommitCommand>, IMercurialCommand<RevSpec>
    {
        /// <summary>
        /// This field is used to specify the encoding of the commit message.
        /// </summary>
        private static readonly Encoding _Encoding = Encoding.GetEncoding("Windows-1252");

        /// <summary>
        /// This is the backing field for the <see cref="Paths"/> property.
        /// </summary>
        private readonly ListFile _Paths = new ListFile();

        /// <summary>
        /// This is the backing field for the <see cref="Message"/> property.
        /// </summary>
        private string _Message = string.Empty;

        /// <summary>
        /// This field is used internally to temporarily hold the filename of the file
        /// that the <see cref="Message"/> was stored into, during command execution.
        /// </summary>
        private string _MessageFilePath;

        /// <summary>
        /// This is the backing field for the <see cref="OverrideAuthor"/> property.
        /// </summary>
        private string _OverrideAuthor = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommitCommand"/> class.
        /// </summary>
        public CommitCommand()
            : base("commit")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the commit message to use when committing.
        /// </summary>
        [DefaultValue("")]
        public string Message
        {
            get
            {
                return _Message;
            }

            set
            {
                _Message = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically add new files and remove missing files before committing.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--addremove")]
        [DefaultValue(false)]
        public bool AddRemove
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to mark a branch as closed, hiding it from the branch list.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--close-branch")]
        [DefaultValue(false)]
        public bool CloseBranch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the username to use when committing;
        /// or <see cref="string.Empty"/> to use the username configured in the repository or by
        /// the current user. Default is <see cref="string.Empty"/>.
        /// </summary>
        [NullableArgument(NonNullOption = "--user")]
        [DefaultValue("")]
        public string OverrideAuthor
        {
            get
            {
                return _OverrideAuthor;
            }

            set
            {
                _OverrideAuthor = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Gets or sets the timestamp <see cref="DateTime"/> to use when committing;
        /// or <c>null</c> which means use the current date and time. Default is <c>null</c>.
        /// </summary>
        [DateTimeArgument(NonNullOption = "--date")]
        [DefaultValue(null)]
        public DateTime? OverrideTimestamp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of files to commit. If left empty, will commit all
        /// pending changes.
        /// </summary>
        public Collection<string> Paths
        {
            get
            {
                return _Paths.Collection;
            }
        }

        #region IMercurialCommand<RevSpec> Members

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        /// <value></value>
        public override IEnumerable<string> Arguments
        {
            get
            {
                return base.Arguments.Concat(
                    new[]
                    {
                        "--logfile", string.Format(CultureInfo.InvariantCulture, "\"{0}\"", _MessageFilePath),
                    }).Concat(_Paths.GetArguments());
            }
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The 'commit' command requires <see cref="Message"/> to be specified.
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (StringEx.IsNullOrWhiteSpace(Message))
                throw new InvalidOperationException("The 'commit' command requires Message to be specified");
            DebugOutput = true;
        }

        /// <summary>
        /// Gets or sets the result of executing the command as a <see cref="RevSpec"/> identifying the
        /// new changeset that was committed.
        /// </summary>
        public RevSpec Result
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Sets the <see cref="Message"/> property to the specified value and
        /// returns this <see cref="CommitCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Message"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CommitCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CommitCommand WithMessage(string value)
        {
            Message = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="AddRemove"/> property to the specified value and
        /// returns this <see cref="CommitCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="AddRemove"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="CommitCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CommitCommand WithAddRemove(bool value = true)
        {
            AddRemove = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="CloseBranch"/> property to the specified value and
        /// returns this <see cref="CommitCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="CloseBranch"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="CommitCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CommitCommand WithCloseBranch(bool value = true)
        {
            CloseBranch = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OverrideAuthor"/> property to the specified value and
        /// returns this <see cref="CommitCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="OverrideAuthor"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CommitCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CommitCommand WithOverrideAuthor(string value)
        {
            OverrideAuthor = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="OverrideTimestamp"/> property to the specified value and
        /// returns this <see cref="CommitCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="OverrideTimestamp"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="CommitCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public CommitCommand WithOverrideTimestamp(DateTime value)
        {
            OverrideTimestamp = value;
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="Paths"/> collection property and
        /// returns this <see cref="ForgetCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Paths"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="ForgetCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="value"/> is <c>null</c> or empty.</para>
        /// </exception>
        public CommitCommand WithPath(string value)
        {
            if (StringEx.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            Paths.Add(value.Trim());
            return this;
        }

        /// <summary>
        /// Override this method to implement code that will execute before command
        /// line execution.
        /// </summary>
        protected override void Prepare()
        {
            _MessageFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Replace("-", string.Empty).ToLowerInvariant() + ".txt");
            File.WriteAllText(_MessageFilePath, Message, _Encoding);
        }

        /// <summary>
        /// Override this method to implement code that will execute after command
        /// line execution.
        /// </summary>
        protected override void Cleanup()
        {
            if (_MessageFilePath != null && File.Exists(_MessageFilePath))
                File.Delete(_MessageFilePath);
            _Paths.Cleanup();
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
            var re = new Regex(@"^committed\s+changeset\s+\d+:(?<hash>[0-9a-f]{40})$", RegexOptions.IgnoreCase);

            foreach (Match ma in standardOutput.Split(
                new[]
                {
                    '\n', '\r'
                }, StringSplitOptions.RemoveEmptyEntries).Select(line => re.Match(line)).Where(ma => ma.Success))
            {
                Result = RevSpec.Single(ma.Groups["hash"].Value);
                return;
            }

            Result = null;
        }
    }
}
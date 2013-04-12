using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg status" command (<see href="http://www.selenic.com/mercurial/hg.1.html#status"/>):
    /// show changed files in the working directory.
    /// </summary>
    public sealed class StatusCommand : MercurialCommandBase<StatusCommand>, IMercurialCommand<IEnumerable<FileStatus>>
    {
        /// <summary>
        /// This dictionary is used internally to map between the output from the
        /// "hg status" command and the various <see cref="FileState"/> enum values.
        /// </summary>
        private static readonly Dictionary<char, FileState> _FileStateCodes = new Dictionary<char, FileState>
        {
            { 'M', FileState.Modified },
            { 'A', FileState.Added },
            { 'R', FileState.Removed },
            { 'C', FileState.Clean },
            { '!', FileState.Missing },
            { '?', FileState.Unknown },
            { 'I', FileState.Ignored },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCommand"/> class.
        /// </summary>
        public StatusCommand()
            : base("status")
        {
            Include = FileStatusIncludes.Default;
        }

        /// <summary>
        /// Gets or sets a value indicating which kind of status codes to include. Default is
        /// <see cref="FileStatusIncludes.Default"/>.
        /// </summary>
        [DefaultValue(FileStatusIncludes.Default)]
        [EnumArgument(FileStatusIncludes.Added, "--added", IsBitmask = true)]
        [EnumArgument(FileStatusIncludes.Clean, "--clean", IsBitmask = true)]
        [EnumArgument(FileStatusIncludes.Ignored, "--ignored", IsBitmask = true)]
        [EnumArgument(FileStatusIncludes.Missing, "--deleted", IsBitmask = true)]
        [EnumArgument(FileStatusIncludes.Modified, "--modified", IsBitmask = true)]
        [EnumArgument(FileStatusIncludes.Removed, "--removed", IsBitmask = true)]
        [EnumArgument(FileStatusIncludes.Unknown, "--unknown", IsBitmask = true)]
        public FileStatusIncludes Include
        {
            get;
            set;
        }

        #region IMercurialCommand<IEnumerable<FileStatus>> Members

        /// <summary>
        /// Gets the result of executing the command as a collection of <see cref="FileStatus"/> objects.
        /// </summary>
        public IEnumerable<FileStatus> Result
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Sets the <see cref="Include"/> property to the specified value and
        /// returns this <see cref="StatusCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Include"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="StatusCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public StatusCommand WithInclude(FileStatusIncludes value)
        {
            Include = value;
            return this;
        }

        /// <summary>
        /// Parses the standard output for results.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        /// <param name="standardOutput">The standard output.</param>
        /// <exception cref="InvalidOperationException">
        /// <para>Status does not yet support the Added sub-state to show where the file was added from.</para>
        /// <para>- or -</para>
        /// <para>An unknown status character was detected in the command output.</para>
        /// </exception>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            base.ParseStandardOutputForResults(exitCode, standardOutput);

            var result = new List<FileStatus>();

            var re = new Regex(@"^(?<status>[MARC!?I ])\s+(?<path>.*)$");
            var statusEntries = from line in standardOutput.Split('\n', '\r')
                                where !StringEx.IsNullOrWhiteSpace(line)
                                let ma = re.Match(line)
                                where ma.Success
                                select new
                                {
                                    status = ma.Groups["status"].Value[0],
                                    path = ma.Groups["path"].Value
                                };
            foreach (var entry in statusEntries)
            {
                FileState state;
                if (_FileStateCodes.TryGetValue(entry.status, out state))
                    result.Add(new FileStatus(state, entry.path));
                else
                {
                    if (entry.status == ' ')
                        throw new InvalidOperationException("Status does not yet support the Added sub-state to show where the file was added from");
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture, "Unknown status code reported by Mercurial: '{0}', I do not know how to handle that",
                            entry.status));
                }
            }

            Result = result;
        }
    }
}
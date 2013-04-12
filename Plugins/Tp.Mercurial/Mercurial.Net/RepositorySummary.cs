using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Mercurial
{
    /// <summary>
    /// This class contains a summary of the repository state, as returned by the
    /// <see cref="SummaryCommand"/>.
    /// </summary>
    public class RepositorySummary
    {
        /// <summary>
        /// This is the backing field for the <see cref="ParentRevisionNumbers"/> property.
        /// </summary>
        private readonly List<int> _ParentRevisionNumbers = new List<int>();

        /// <summary>
        /// Parses the standard output from executing a 'hg summary' command
        /// and returns a <see cref="RepositorySummary"/>.
        /// </summary>
        /// <param name="standardOutput">
        /// The standard output from a 'hg summary' command that is to be parsed.
        /// </param>
        /// <returns>
        /// The resulting <see cref="RepositorySummary"/> from parsing <paramref name="standardOutput"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="standardOutput"/> is <c>null</c>.</para>
        /// </exception>
        public static RepositorySummary Parse(string standardOutput)
        {
            if (standardOutput == null)
                throw new ArgumentNullException("standardOutput");

            var parsers = new List<KeyValuePair<Regex, Action<RepositorySummary, string, Match>>>
            {
                new KeyValuePair<Regex, Action<RepositorySummary, string, Match>>(
                    new Regex(@"^parent:\s(?<revnum>-?\d+):[a-f0-9]+\s+.*$", RegexOptions.IgnoreCase),
                    ParseParents),
                new KeyValuePair<Regex, Action<RepositorySummary, string, Match>>(
                    new Regex(@"^branch:\s(?<name>.*)$", RegexOptions.IgnoreCase),
                    ParseBranch),
                new KeyValuePair<Regex, Action<RepositorySummary, string, Match>>(
                    new Regex(@"^update:\s((?<new>\d+)\snew changesets? \(update\))$", RegexOptions.IgnoreCase),
                    ParseUpdate),
                new KeyValuePair<Regex, Action<RepositorySummary, string, Match>>(
                    new Regex(@"^commit:\s((?<modified>\d+) modified(, )?)?((?<unknown>\d+) unknown(, )?)?((?<unresolved>\d+) unresolved(, )?)?(?<inmerge> \(merge\))?( ?\(clean\))?$", RegexOptions.IgnoreCase),
                    ParseCommit)
            };

            var summary = new RepositorySummary { RawOutput = standardOutput };
            foreach (string line in OutputParsingUtilities.SplitIntoLines(standardOutput))
            {
                foreach (var parser in parsers)
                {
                    Match ma = parser.Key.Match(line);
                    if (!ma.Success)
                        continue;
                    
                    parser.Value(summary, line, ma);
                }
            }
            return summary;
        }

        /// <summary>
        /// Parses the <c>branch: </c> line from the summary command.
        /// </summary>
        /// <param name="summary">
        /// The <see cref="RepositorySummary"/> instance to write the parsed information into.
        /// </param>
        /// <param name="line">
        /// The line for which the parse method was called.
        /// </param>
        /// <param name="match">
        /// The <see cref="Match"/> object resulting in matching up a regular expression with the line.
        /// </param>
        private static void ParseBranch(RepositorySummary summary, string line, Match match)
        {
            summary.Branch = match.Groups["name"].Value;
        }

        /// <summary>
        /// Parses the <c>parent: </c> line from the summary command.
        /// </summary>
        /// <param name="summary">
        /// The <see cref="RepositorySummary"/> instance to write the parsed information into.
        /// </param>
        /// <param name="line">
        /// The line for which the parse method was called.
        /// </param>
        /// <param name="match">
        /// The <see cref="Match"/> object resulting in matching up a regular expression with the line.
        /// </param>
        private static void ParseParents(RepositorySummary summary, string line, Match match)
        {
            int revisionNumber = int.Parse(match.Groups["revnum"].Value, CultureInfo.InvariantCulture);
            summary._ParentRevisionNumbers.Add(revisionNumber);
        }

        /// <summary>
        /// Parses the <c>update: </c> line from the summary command.
        /// </summary>
        /// <param name="summary">
        /// The <see cref="RepositorySummary"/> instance to write the parsed information into.
        /// </param>
        /// <param name="line">
        /// The line for which the parse method was called.
        /// </param>
        /// <param name="match">
        /// The <see cref="Match"/> object resulting in matching up a regular expression with the line.
        /// </param>
        private static void ParseUpdate(RepositorySummary summary, string line, Match match)
        {
            if (match.Groups["current"].Success)
                summary.NumberOfNewChangesets = 0;
            else if (match.Groups["new"].Success)
                summary.NumberOfNewChangesets = int.Parse(match.Groups["new"].Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses the <c>commit: </c> line from the summary command.
        /// </summary>
        /// <param name="summary">
        /// The <see cref="RepositorySummary"/> instance to write the parsed information into.
        /// </param>
        /// <param name="line">
        /// The line for which the parse method was called.
        /// </param>
        /// <param name="match">
        /// The <see cref="Match"/> object resulting in matching up a regular expression with the line.
        /// </param>
        private static void ParseCommit(RepositorySummary summary, string line, Match match)
        {
            if (match.Groups["modified"].Success)
                summary.NumberOfModifiedFiles = int.Parse(match.Groups["modified"].Value, CultureInfo.InvariantCulture);
            if (match.Groups["unknown"].Success)
                summary.NumberOfUnknownFiles = int.Parse(match.Groups["unknown"].Value, CultureInfo.InvariantCulture);
            if (match.Groups["unresolved"].Success)
                summary.NumberOfUnresolvedFiles = int.Parse(match.Groups["unresolved"].Value, CultureInfo.InvariantCulture);
            summary.IsInMerge = match.Groups["inmerge"].Success;
        }

        /// <summary>
        /// Gets the raw output from the <c>hg summary</c> command.
        /// </summary>
        public string RawOutput
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection of parent revision numbers for the current working folder.
        /// </summary>
        public IEnumerable<int> ParentRevisionNumbers
        {
            get
            {
                return _ParentRevisionNumbers;
            }
        }

        /// <summary>
        /// Gets the name of the branch the current working folder is on.
        /// </summary>
        /// <remarks>
        /// Note that after executing a <c>hg branch XYZ</c> command, the summary command will report XYZ, and
        /// not the branch name the working folder had before any changes were done to it. There is apparently
        /// no way to know if the name reported is the old or the new name.
        /// </remarks>
        public string Branch
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether an update is possible to perform, basically the working
        /// directory is not at the head.
        /// </summary>
        public bool UpdatePossible
        {
            get
            {
                return NumberOfNewChangesets > 0;
            }
        }

        /// <summary>
        /// Gets the number of new changesets that exists in the history following the one that the
        /// working state is at.
        /// </summary>
        public int NumberOfNewChangesets
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of modified files in the working directory.
        /// </summary>
        public int NumberOfModifiedFiles
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of unknown files in the working directory.
        /// </summary>
        public int NumberOfUnknownFiles
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of unresolved files in the currently pending merge.
        /// </summary>
        public int NumberOfUnresolvedFiles
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the current working directory is currently in a pending merge.
        /// </summary>
        public bool IsInMerge
        {
            get;
            private set;
        }
    }
}
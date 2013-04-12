using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg tags" command (<see href="http://www.selenic.com/mercurial/hg.1.html#tags"/>):
    /// list repository tags.
    /// </summary>
    public sealed class TagsCommand : MercurialCommandBase<TagsCommand>, IMercurialCommand<IEnumerable<Tag>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagsCommand"/> class.
        /// </summary>
        public TagsCommand()
            : base("tags")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public IEnumerable<Tag> Result
        {
            get;
            private set;
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
        /// <exception cref="MercurialResultParsingException">
        /// <para><paramref name="standardOutput"/> contains output with invalid/unknown format.</para>
        /// </exception>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            string[] lines = OutputParsingUtilities.SplitIntoLines(standardOutput);

            var re = new Regex(@"^(?<name>.*)\s+(?<revno>-?\d+):[a-f0-9]+$", RegexOptions.IgnoreCase);
            var tags = new List<Tag>();
            foreach (Match ma in lines.Where(l => !StringEx.IsNullOrWhiteSpace(l)).Select(line => re.Match(line)))
            {
                if (!ma.Success)
                    throw new MercurialResultParsingException(exitCode, "Unable to parse output from the tags command", standardOutput);

                tags.Add(new Tag(
                    int.Parse(ma.Groups["revno"].Value, CultureInfo.InvariantCulture),
                    ma.Groups["name"].Value.Trim()));
            }

            Result = tags.OrderBy(b => b.RevisionNumber).ToArray();
        }
    }
}
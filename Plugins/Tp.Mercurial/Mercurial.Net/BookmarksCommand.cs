using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg bookmarks" command (<see href="http://www.selenic.com/mercurial/hg.1.html#bookmarks"/>):
    /// list existing bookmarks.
    /// </summary>
    /// <remarks>
    /// Note that in Mercurial, the "bookmark" and "bookmarks" commands are synonyms for each other, which means that
    /// in Mercurial, to get a list of bookmarks, you execute either without arguments, and to create a new bookmark
    /// you execute either with a name argument. In Mercurial.Net I decided to split these two into separate commands,
    /// so that the <see cref="BookmarksCommand"/> lists existing bookmarks, and the <see cref="BookmarkCommand"/>
    /// creates or deletes bookmarks.
    /// </remarks>
    public sealed class BookmarksCommand : MercurialCommandBase<BookmarksCommand>, IMercurialCommand<IEnumerable<Bookmark>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarksCommand"/> class.
        /// </summary>
        public BookmarksCommand()
            : base("bookmarks")
        {
            // Do nothing here
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

            var re = new Regex(@"^ (?<wf>\*)? +(?<name>.*)\s+(?<revno>-?\d+):[a-f0-9]+$", RegexOptions.IgnoreCase);
            var bookmarks = new List<Bookmark>();
            foreach (Match ma in lines.Where(l => !StringEx.IsNullOrWhiteSpace(l)).Select(line => re.Match(line)))
            {
                if (!ma.Success)
                    throw new MercurialResultParsingException(exitCode, "Unable to parse output from the bookmarks command", standardOutput);

                bookmarks.Add(new Bookmark(
                    int.Parse(ma.Groups["revno"].Value, CultureInfo.InvariantCulture),
                    ma.Groups["name"].Value.Trim(),
                    ma.Groups["wf"].Success));
            }

            Result = bookmarks.OrderBy(b => b.RevisionNumber).ToArray();
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public IEnumerable<Bookmark> Result
        {
            get;
            private set;
        }
    }
}
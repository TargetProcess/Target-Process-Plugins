using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg paths" command (<see href="http://www.selenic.com/mercurial/hg.1.html#paths"/>):
    /// show aliases for remote repositories.
    /// </summary>
    public sealed class PathsCommand : MercurialCommandBase<PathsCommand>, IMercurialCommand<IEnumerable<RemoteRepositoryPath>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathsCommand"/> class.
        /// </summary>
        public PathsCommand()
            : base("paths")
        {
            // Do nothing here
        }

        #region IMercurialCommand<IEnumerable<RemoteRepositoryPath>> Members

        /// <summary>
        /// Gets the result of executing the command as a collection of <see cref="RemoteRepositoryPath"/> objects.
        /// </summary>
        public IEnumerable<RemoteRepositoryPath> Result
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Parses the standard output for results.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        /// <param name="standardOutput">The standard output.</param>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            base.ParseStandardOutputForResults(exitCode, standardOutput);

            var result = new List<RemoteRepositoryPath>();
            var re = new Regex(@"^(?<name>[^=]+)\s*=\s*(?<path>.*)$", RegexOptions.None);
            using (var reader = new StringReader(standardOutput))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Match ma = re.Match(line);
                    if (ma.Success)
                        result.Add(new RemoteRepositoryPath(ma.Groups["name"].Value.Trim(), ma.Groups["path"].Value.Trim()));
                }
            }
            Result = result;
        }
    }
}
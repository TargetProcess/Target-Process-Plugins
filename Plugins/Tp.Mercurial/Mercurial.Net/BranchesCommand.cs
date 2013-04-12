using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg branches" command (<see href="http://www.selenic.com/mercurial/hg.1.html#branches"/>):
    /// list repository named branches.
    /// </summary>
    public sealed class BranchesCommand : MercurialCommandBase<BranchesCommand>, IMercurialCommand<IEnumerable<BranchHead>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchesCommand"/> class.
        /// </summary>
        public BranchesCommand()
            : base("branches")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show only branches that have heads.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--active")]
        [DefaultValue(false)]
        public bool OnlyActive
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="OnlyActive"/> property to the specified value and
        /// returns this <see cref="BranchesCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="OnlyActive"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="BranchesCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BranchesCommand WithOnlyActive(bool value = true)
        {
            OnlyActive = value;
            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show normal and closed branches.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--closed")]
        [DefaultValue(false)]
        public bool IncludeClosedBranches
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="IncludeClosedBranches"/> property to the specified value and
        /// returns this <see cref="BranchesCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="IncludeClosedBranches"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="BranchesCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BranchesCommand WithIncludeClosedBranches(bool value = true)
        {
            IncludeClosedBranches = value;
            return this;
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public IEnumerable<BranchHead> Result
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

            var re = new Regex(@"^(?<name>.*)\s+(?<revno>-?\d+):[a-f0-9]+(\s+\(inactive\))?$", RegexOptions.IgnoreCase);
            var branchHeads = new List<BranchHead>();
            foreach (Match ma in lines.Where(l => !StringEx.IsNullOrWhiteSpace(l)).Select(line => re.Match(line)))
            {
                if (!ma.Success)
                    throw new MercurialResultParsingException(exitCode, "Unable to parse output from the branches command", standardOutput);

                branchHeads.Add(new BranchHead(
                    int.Parse(ma.Groups["revno"].Value, CultureInfo.InvariantCulture),
                    ma.Groups["name"].Value.Trim()));
            }

            Result = branchHeads.OrderBy(b => b.Name).ToArray();
        }
    }
}
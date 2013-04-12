using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg summary" command (<see href="http://www.selenic.com/mercurial/hg.1.html#summary"/>):
    /// summarize working directory state.
    /// </summary>
    public sealed class SummaryCommand : MercurialCommandBase<SummaryCommand>, IMercurialCommand<RepositorySummary>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryCommand"/> class.
        /// </summary>
        public SummaryCommand()
            : base("summary")
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
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to call
        /// the base method at all.
        /// </remarks>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            if (exitCode == 0)
                Result = RepositorySummary.Parse(standardOutput);
            else
                base.ParseStandardOutputForResults(exitCode, standardOutput);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to check for push and pull against the
        /// default remote repository.
        /// </summary>
        [BooleanArgument(TrueOption = "--remote")]
        [DefaultValue(false)]
        public bool CheckPushAndPull
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="CheckPushAndPull"/> property to the specified value and
        /// returns this <see cref="SummaryCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="CheckPushAndPull"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="SummaryCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public SummaryCommand WithCheckPushAndPull(bool value)
        {
            CheckPushAndPull = value;
            return this;
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public RepositorySummary Result
        {
            get;
            private set;
        }
    }
}
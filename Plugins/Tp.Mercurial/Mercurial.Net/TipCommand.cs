using System.Collections.Generic;
using System.Linq;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg tip" command (<see href="http://www.selenic.com/mercurial/hg.1.html#tip"/>):
    /// show the tip revision.
    /// </summary>
    public sealed class TipCommand : MercurialCommandBase<TipCommand>, IMercurialCommand<Changeset>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TipCommand"/> class.
        /// </summary>
        public TipCommand()
            : base("tip")
        {
        }

        #region IMercurialCommand<Changeset> Members

        /// <summary>
        /// Gets the result of executing the command as a <see cref="Changeset"/> object.
        /// </summary>
        public Changeset Result
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        /// <remarks>
        /// Note that as long as you descend from <see cref="MercurialCommandBase{T}"/> you're not required to access
        /// the base property at all, but you are required to return a non-<c>null</c> array reference,
        /// even for an empty array.
        /// </remarks>
        public override IEnumerable<string> Arguments
        {
            get
            {
                return base.Arguments.Concat(
                    new[]
                    {
                        "--style=XML"
                    });
            }
        }

        #endregion

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
            Result = ChangesetXmlParser.Parse(standardOutput).FirstOrDefault();
        }
    }
}
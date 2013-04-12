using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg parents" command (<see href="http://www.selenic.com/mercurial/hg.1.html#parents"/>):
    /// show the parents of the working directory or revision.
    /// </summary>
    public sealed class ParentsCommand : MercurialCommandBase<ParentsCommand>, IMercurialCommand<IEnumerable<Changeset>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParentsCommand"/> class.
        /// </summary>
        public ParentsCommand()
            : base("parents")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> to get the revision of.
        /// </summary>
        [NullableArgument(NonNullOption = "--rev")]
        [DefaultValue(null)]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this <see cref="ParentsCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ParentsCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ParentsCommand WithRevision(RevSpec value)
        {
            Revision = value;
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
            Result = ChangesetXmlParser.Parse(standardOutput);
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public IEnumerable<Changeset> Result
        {
            get;
            private set;
        }

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
    }
}
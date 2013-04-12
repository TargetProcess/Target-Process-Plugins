using System.Collections.Generic;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg manifest" command (<see href="http://www.selenic.com/mercurial/hg.1.html#manifest"/>):
    /// output the current or given revision of the project manifest.
    /// </summary>
    public sealed class ManifestCommand : MercurialCommandBase<ManifestCommand>, IMercurialCommand<IEnumerable<string>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestCommand"/> class.
        /// </summary>
        public ManifestCommand()
            : base("manifest")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> to update the working copy to.
        /// </summary>
        [NullableArgument]
        [DefaultValue(null)]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this <see cref="ManifestCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ManifestCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ManifestCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
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
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            base.ParseStandardOutputForResults(exitCode, standardOutput);

            Result = OutputParsingUtilities.SplitIntoLines(standardOutput);
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public IEnumerable<string> Result
        {
            get;
            private set;
        }
    }
}
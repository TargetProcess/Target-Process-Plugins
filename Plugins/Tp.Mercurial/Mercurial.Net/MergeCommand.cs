using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mercurial.Attributes;
using Mercurial.Versions;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg merge" command (<see href="http://www.selenic.com/mercurial/hg.1.html#merge"/>):
    /// merge working directory with another revision.
    /// </summary>
    public sealed class MergeCommand : MercurialCommandBase<MergeCommand>, IMercurialCommand<MergeResult>
    {
        /// <summary>
        /// This is the backing field for the <see cref="MergeTool"/> property.
        /// </summary>
        private string _MergeTool = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeCommand"/> class.
        /// </summary>
        public MergeCommand()
            : base("merge")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force a merge with outstanding changes.
        /// Default value is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--force")]
        [DefaultValue(false)]
        public bool Force
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Force"/> property to the specified value and
        /// returns this <see cref="MergeCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Force"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="MergeCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public MergeCommand WithForce(bool value)
        {
            Force = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the merge tool to use.
        /// Default value is <see cref="string.Empty"/> in which case the default merge tool(s) are used.
        /// </summary>
        [DefaultValue("")]
        public string MergeTool
        {
            get
            {
                return _MergeTool;
            }

            set
            {
                RequiresVersion(new Version(1, 7), "MergeTool property of the MergeCommand class");
                _MergeTool = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="MergeTool"/> property to the specified value and
        /// returns this <see cref="MergeCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="MergeTool"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="MergeCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public MergeCommand WithMergeTool(string value)
        {
            MergeTool = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the <see cref="RevSpec"/> to merge the working folder with.
        /// Default value is <c>null</c> in which case the Mercurial client will try to figure it out
        /// itself.
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
        /// returns this <see cref="MergeCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="MergeCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public MergeCommand WithRevision(RevSpec value)
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
            switch (exitCode)
            {
                case 0:
                    Result = MergeResult.Success;
                    break;

                case 1:
                    Result = MergeResult.UnresolvedFiles;
                    break;

                default:
                    base.ParseStandardOutputForResults(exitCode, standardOutput);
                    break;
            }
        }

        /// <summary>
        /// This method should throw the appropriate exception depending on the contents of
        /// the <paramref name="exitCode"/> and <paramref name="standardErrorOutput"/>
        /// parameters, or simply return if the execution is considered successful.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code from executing the command line client.
        /// </param>
        /// <param name="standardErrorOutput">
        /// The standard error output from executing the command client.
        /// </param>
        /// <exception cref="MercurialExecutionException">
        /// <para><paramref name="exitCode"/> is not <c>0</c>.</para>
        /// </exception>
        protected override void ThrowOnUnsuccessfulExecution(int exitCode, string standardErrorOutput)
        {
            if (exitCode != 1)
                base.ThrowOnUnsuccessfulExecution(exitCode, standardErrorOutput);
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public MergeResult Result
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all the arguments to the <see cref="CommandBase{T}.Command"/>, or an
        /// empty array if there are none.
        /// </summary>
        public override IEnumerable<string> Arguments
        {
            get
            {
                foreach (string argument in GetBaseArguments())
                    yield return argument;

                foreach (string argument in MercurialVersionBase.Current.MergeToolOption(MergeTool))
                    yield return argument;
            }
        }

        /// <summary>
        /// Gets the base arguments.
        /// </summary>
        /// <returns>
        /// The contents of the base arguments property, to avoide unverifiable code in <see cref="Arguments"/>.
        /// </returns>
        private IEnumerable<string> GetBaseArguments()
        {
            return base.Arguments;
        }
    }
}
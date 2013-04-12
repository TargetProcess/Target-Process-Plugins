using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg bisect" command (<see href="http://www.selenic.com/mercurial/hg.1.html#bisect"/>):
    /// subdivision search of changesets.
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="BisectCommand"/> should be used by first updating the repository to a changeset
    /// where the current state is known to be bad or good, and then updated to another changeset were the
    /// current state is known to be the opposite of the previous changeset. Then the <see cref="BisectCommand"/>
    /// should be iteratively executed, analyzing the <see cref="BisectCommand.Result"/> property each time.</para>
    /// <para>When the <see cref="BisectResult"/> object in that property says <see cref="BisectResult.Done"/> = <c>false</c>,
    /// then the repository has been updated to a new changeset. The program should then analyze the changeset and determine
    /// if its state is good or bad, and issue another <see cref="BisectCommand"/> to that effect.</para>
    /// <para>When the <see cref="BisectResult"/> says that <see cref="BisectResult.Done"/> = <c>true</c>,
    /// the <see cref="BisectResult.Revision"/> property contains the revspec of the first changeset that is
    /// deemed good or bad, depending on what you're looking for.</para>
    /// </remarks>
    public sealed class BisectCommand : MercurialCommandBase<BisectCommand>, IMercurialCommand<BisectResult>
    {
        /// <summary>
        /// This is the backing field for the <see cref="TestCommand"/> property.
        /// </summary>
        private string _TestCommand = string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="State"/> property.
        /// </summary>
        private BisectState _State = BisectState.None;

        /// <summary>
        /// This is the backing field for the <see cref="Update"/> property.
        /// </summary>
        private bool _Update = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="BisectCommand"/> class.
        /// </summary>
        public BisectCommand()
            : base("bisect")
        {
        }

        /// <summary>
        /// Gets or sets a value specifying how to proceed with the bisect command, by marking
        /// the current changeset good or bad.
        /// </summary>
        [EnumArgument(BisectState.Skip, "--skip")]
        [EnumArgument(BisectState.Good, "--good")]
        [EnumArgument(BisectState.Bad, "--bad")]
        [EnumArgument(BisectState.Reset, "--reset")]
        [DefaultValue(BisectState.None)]
        public BisectState State
        {
            get
            {
                return _State;
            }

            set
            {
                _State = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to update to target revision.
        /// Default value is <c>true</c>.
        /// </summary>
        [BooleanArgument(FalseOption = "--noupdate")]
        [DefaultValue(true)]
        public bool Update
        {
            get
            {
                return _Update;
            }

            set
            {
                _Update = value;
            }
        }

        /// <summary>
        /// Gets or sets the command to execute from Mercurial in order to test each changeset,
        /// doing an automated search instead of one controlled by the program using Mercurial.Net.
        /// </summary>
        [DefaultValue("")]
        public string TestCommand
        {
            get
            {
                return _TestCommand;
            }

            set
            {
                _TestCommand = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">The State property must be set to something other than None before executing a BisectCommand</exception>
        public override void Validate()
        {
            base.Validate();

            if (State == BisectState.None)
                throw new InvalidOperationException("The State property must be set to something other than None before executing a BisectCommand");
        }

        /// <summary>
        /// Sets the <see cref="State"/> property to the specified value and
        /// returns this <see cref="BisectCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="State"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="BisectCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BisectCommand WithState(BisectState value)
        {
            State = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Update"/> property to the specified value and
        /// returns this <see cref="BisectCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Update"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="BisectCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public BisectCommand WithUpdate(bool value)
        {
            Update = value;
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
        /// <exception cref="MercurialResultParsingException">
        /// <para>There was an error in the output from executing the command; the text did not match any of the known patterns.</para>
        /// </exception>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            if (ParseEmptyResult(standardOutput))
                return;

            if (ParseTestingResult(standardOutput))
                return;

            if (ParseFoundResult(standardOutput))
                return;

            throw new MercurialResultParsingException(exitCode, "Unknown result returned from the bisect command", standardOutput);
        }

        /// <summary>
        /// Attempts to parse the empty result, typically returned from executing a <see cref="BisectCommand"/>
        /// with a <see cref="State"/> of <see cref="BisectState.Reset"/>.
        /// </summary>
        /// <param name="standardOutput">
        /// The standard output from executing the command.
        /// </param>
        /// <returns>
        /// <c>true</c> if this method was able to parse the results correctly;
        /// otherwise <c>false</c> to continue testing other ways to parse it.
        /// </returns>
        private bool ParseEmptyResult(string standardOutput)
        {
            if (StringEx.IsNullOrWhiteSpace(standardOutput))
            {
                Result = new BisectResult();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to parse the results that indicate that the first good changeset was found.
        /// </summary>
        /// <param name="standardOutput">
        /// The standard output from executing the command.
        /// </param>
        /// <returns>
        /// <c>true</c> if this method was able to parse the results correctly;
        /// otherwise <c>false</c> to continue testing other ways to parse it.
        /// </returns>
        private bool ParseFoundResult(string standardOutput)
        {
            var re = new Regex(@"^The first good revision is:\s+changeset:\s+(?<revno>\d+):", RegexOptions.IgnoreCase);
            Match ma = re.Match(standardOutput);
            if (ma.Success)
            {
                int foundAtRevision = int.Parse(ma.Groups["revno"].Value, CultureInfo.InvariantCulture);
                Result = new BisectResult(RevSpec.Single(foundAtRevision), true);
            }

            return ma.Success;
        }

        /// <summary>
        /// Attempts to parse the results indicating that further testing is required and that the
        /// repository has now been updated to a new revision for testing and subsequent marking of
        /// good or bad.
        /// </summary>
        /// <param name="standardOutput">
        /// The standard output from executing the command.
        /// </param>
        /// <returns>
        /// <c>true</c> if this method was able to parse the results correctly;
        /// otherwise <c>false</c> to continue testing other ways to parse it.
        /// </returns>
        private bool ParseTestingResult(string standardOutput)
        {
            var re = new Regex(@"^Testing changeset (?<revno>\d+):", RegexOptions.IgnoreCase);
            Match ma = re.Match(standardOutput);
            if (ma.Success)
            {
                int currentlyAtRevision = int.Parse(ma.Groups["revno"].Value, CultureInfo.InvariantCulture);
                Result = new BisectResult(RevSpec.Single(currentlyAtRevision), false);
            }

            return ma.Success;
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public BisectResult Result
        {
            get;
            private set;
        }
    }
}
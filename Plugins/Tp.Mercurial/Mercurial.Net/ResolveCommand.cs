using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Mercurial.Attributes;
using Mercurial.Versions;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg resolve" command (<see href="http://www.selenic.com/mercurial/hg.1.html#resolve"/>):
    /// Redo merges or set/view the merge status of files.
    /// </summary>
    public sealed class ResolveCommand : IncludeExcludeCommandBase<ResolveCommand>, IMercurialCommand<IEnumerable<MergeConflict>>
    {
        /// <summary>
        /// This is the backing field for the <see cref="Files"/> property.
        /// </summary>
        private readonly ListFile _Files = new ListFile();

        /// <summary>
        /// This is the backing field for the <see cref="MergeTool"/> property.
        /// </summary>
        private string _MergeTool = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveCommand"/> class.
        /// </summary>
        public ResolveCommand()
            : base("resolve")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to select all unresolved files.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Note that if the <see cref="Action"/> property is set to <see cref="ResolveAction.List"/>, then
        /// this property is not used.
        /// </remarks>
        [BooleanArgument(TrueOption = "--all")]
        [DefaultValue(false)]
        public bool SelectAll
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="SelectAll"/> property to the specified value and
        /// returns this <see cref="ResolveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="SelectAll"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ResolveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ResolveCommand WithSelectAll(bool value = true)
        {
            SelectAll = value;
            return this;
        }

        /// <summary>
        /// Gets the collection of files to process.
        /// </summary>
        public Collection<string> Files
        {
            get
            {
                return _Files.Collection;
            }
        }

        /// <summary>
        /// Adds the value to the <see cref="Files"/> collection property and
        /// returns this <see cref="ResolveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="Files"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="ResolveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="value"/> is <c>null</c> or empty.</para>
        /// </exception>
        public ResolveCommand WithFile(string value)
        {
            if (StringEx.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            Files.Add(value);
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
                RequiresVersion(new Version(1, 7), "MergeTool property of the ResolveCommand class");
                _MergeTool = (value ?? string.Empty).Trim();
            }
        }

        /// <summary>
        /// Sets the <see cref="MergeTool"/> property to the specified value and
        /// returns this <see cref="ResolveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="MergeTool"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ResolveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ResolveCommand WithMergeTool(string value)
        {
            MergeTool = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the <see cref="ResolveAction"/> to take.
        /// Default value is <see cref="ResolveAction.MarkResolved"/>.
        /// </summary>
        [EnumArgument(ResolveAction.MarkResolved, "--mark")]
        [EnumArgument(ResolveAction.MarkUnresolved, "--unmark")]
        [EnumArgument(ResolveAction.List, "--list")]
        [DefaultValue(ResolveAction.MarkResolved)]
        public ResolveAction Action
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="Action"/> property to the specified value and
        /// returns this <see cref="ResolveCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Action"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="ResolveCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public ResolveCommand WithAction(ResolveAction value)
        {
            Action = value;
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
        /// Unable to parse one or more of the lines of output from the 'hg resolve --list' command
        /// </exception>
        protected override void ParseStandardOutputForResults(int exitCode, string standardOutput)
        {
            if (exitCode != 0 || Action != ResolveAction.List)
                return;
            
            var resolutionRegex = new Regex(@"^(?<state>[UR])\s(?<file>.*)$", RegexOptions.IgnoreCase);

            string[] lines = OutputParsingUtilities.SplitIntoLines(standardOutput);
            var result = new List<MergeConflict>();
            foreach (Match resolutionMatch in lines.Select(line => resolutionRegex.Match(line)))
            {
                if (!resolutionMatch.Success)
                    throw new MercurialResultParsingException(exitCode, "Unable to parse one or more of the lines of output from the 'hg resolve --list' command", standardOutput);

                result.Add(new MergeConflict(
                    resolutionMatch.Groups["file"].Value,
                    resolutionMatch.Groups["state"].Value == "R" ? MergeConflictState.Resolved : MergeConflictState.Unresolved));
            }
            Result = result;
        }

        /// <summary>
        /// Gets the result from the command line execution, as an appropriately typed value.
        /// </summary>
        public IEnumerable<MergeConflict> Result
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

                foreach (string argument in _Files.GetArguments())
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

        /// <summary>
        /// Override this method to implement code that will execute after command
        /// line execution.
        /// </summary>
        protected override void Cleanup()
        {
            base.Cleanup();
            _Files.Cleanup();
        }
    }
}
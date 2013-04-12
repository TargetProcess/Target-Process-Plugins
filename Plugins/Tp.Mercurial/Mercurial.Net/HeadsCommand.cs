using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mercurial.Attributes;

namespace Mercurial
{
    /// <summary>
    /// This class implements the "hg heads" command (<see href="http://www.selenic.com/mercurial/hg.1.html#heads"/>):
    /// show current repository heads or show branch heads.
    /// </summary>
    public sealed class HeadsCommand : MercurialCommandBase<HeadsCommand>, IMercurialCommand<IEnumerable<Changeset>>
    {
        /// <summary>
        /// This is the backing field for the <see cref="BranchRevisions"/> property.
        /// </summary>
        private readonly List<RevSpec> _BranchRevisions = new List<RevSpec>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadsCommand"/> class.
        /// </summary>
        public HeadsCommand()
            : base("heads")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets the <see cref="Revision"/> to get heads of, if non-<c>null</c> will
        /// only include heads that are descendants of the specified revision.
        /// Default is <c>null</c>.
        /// </summary>
        [NullableArgument(NonNullOption = "--rev")]
        [DefaultValue(null)]
        public RevSpec Revision
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show topological branches only (branches without children.)
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--topo")]
        [DefaultValue(false)]
        public bool Topological
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include closed branches in the collection.
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--closed")]
        [DefaultValue(false)]
        public bool ShowClosed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of <see cref="Revision"/>, if non-empty, will only
        /// include heads associated with the branches these revisions are on.
        /// Default is empty.
        /// </summary>
        public Collection<RevSpec> BranchRevisions
        {
            get
            {
                return new Collection<RevSpec>(_BranchRevisions);
            }
        }

        #region IMercurialCommand<IEnumerable<Changeset>> Members

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
                    "--style=XML"
                };
                return arguments.Concat(base.Arguments).ToArray();
            }
        }

        /// <summary>
        /// Gets the result of executing the command as a collection of <see cref="Changeset"/> objects.
        /// </summary>
        public IEnumerable<Changeset> Result
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Sets the <see cref="Revision"/> property to the specified value and
        /// returns this <see cref="HeadsCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Revision"/> property.
        /// </param>
        /// <returns>
        /// This <see cref="HeadsCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public HeadsCommand WithRevision(RevSpec value)
        {
            Revision = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Topological"/> property to the specified value and
        /// returns this <see cref="HeadsCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="Topological"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="HeadsCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public HeadsCommand WithTopological(bool value = true)
        {
            Topological = value;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="ShowClosed"/> property to the specified value and
        /// returns this <see cref="HeadsCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="ShowClosed"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="HeadsCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public HeadsCommand WithShowClosed(bool value = true)
        {
            ShowClosed = value;
            return this;
        }

        /// <summary>
        /// Adds the value to the <see cref="BranchRevisions"/> collection property and
        /// returns this <see cref="HeadsCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The value to add to the <see cref="BranchRevisions"/> collection property.
        /// </param>
        /// <returns>
        /// This <see cref="HeadsCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public HeadsCommand WithBranchRevision(RevSpec value)
        {
            BranchRevisions.Add(value);
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
    }
}
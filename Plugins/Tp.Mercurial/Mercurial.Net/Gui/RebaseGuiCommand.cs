using System;
using System.ComponentModel;
using Mercurial.Attributes;

namespace Mercurial.Gui
{
    /// <summary>
    /// Implements the TortoiseHg "rebase" command:
    /// Rebase changesets.
    /// </summary>
    /// <remarks>
    /// This command is only available for the <see cref="GuiClientType.PyQT"/> client type.
    /// </remarks>
    [GuiClientRequirement(ClientType = GuiClientType.PyQT)]
    public sealed class RebaseGuiCommand : GuiCommandBase<RebaseGuiCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RebaseGuiCommand"/> class.
        /// </summary>
        public RebaseGuiCommand()
            : base("rebase")
        {
            // Do nothing here
        }

        /// <summary>
        /// Gets or sets a value indicating whether to keep the original changesets (ie. make a copy), or to strip them (ie. move them.)
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--keep")]
        [DefaultValue(false)]
        public bool KeepOriginalChangesets
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="KeepOriginalChangesets"/> property to the specified value and
        /// returns this <see cref="RebaseGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="KeepOriginalChangesets"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="RebaseGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RebaseGuiCommand WithKeepOriginalChangesets(bool value = true)
        {
            KeepOriginalChangesets = value;
            return this;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to keep the original changesets (ie. make a copy), or to strip them (ie. move them.)
        /// Default is <c>false</c>.
        /// </summary>
        [BooleanArgument(TrueOption = "--keep")]
        [DefaultValue(false)]
        public bool DetachFromOriginalBranch
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="DetachFromOriginalBranch"/> property to the specified value and
        /// returns this <see cref="RebaseGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="DetachFromOriginalBranch"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="RebaseGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RebaseGuiCommand WithDetachFromOriginalBranch(bool value = true)
        {
            DetachFromOriginalBranch = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the source revision to rebase. This changeset, and all its descendants, will be rebased.
        /// Default is <c>null</c>.
        /// </summary>
        [NullableArgument(NonNullOption = "--source")]
        [DefaultValue(null)]
        public RevSpec SourceRevision
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="SourceRevision"/> property to the specified value and
        /// returns this <see cref="RebaseGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="SourceRevision"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="RebaseGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RebaseGuiCommand WithSourceRevision(RevSpec value)
        {
            SourceRevision = value;
            return this;
        }

        /// <summary>
        /// Gets or sets the Destination revision to rebase. This changeset will be the parent of the changesets after the rebase.
        /// Default is <c>null</c>.
        /// </summary>
        [NullableArgument(NonNullOption = "--dest")]
        [DefaultValue(null)]
        public RevSpec DestinationRevision
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the <see cref="DestinationRevision"/> property to the specified value and
        /// returns this <see cref="RebaseGuiCommand"/> instance.
        /// </summary>
        /// <param name="value">
        /// The new value for the <see cref="DestinationRevision"/> property,
        /// defaults to <c>true</c>.
        /// </param>
        /// <returns>
        /// This <see cref="RebaseGuiCommand"/> instance.
        /// </returns>
        /// <remarks>
        /// This method is part of the fluent interface.
        /// </remarks>
        public RebaseGuiCommand WithDestinationRevision(RevSpec value)
        {
            DestinationRevision = value;
            return this;
        }

        /// <summary>
        /// Validates the command configuration. This method should throw the necessary
        /// exceptions to signal missing or incorrect configuration (like attempting to
        /// add files to the repository without specifying which files to add.)
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// <para><see cref="SourceRevision"/> must be set before <see cref="RebaseGuiCommand"/> can be executed.</para>
        /// <para>- or -</para>
        /// <para><see cref="DestinationRevision"/> must be set before <see cref="RebaseGuiCommand"/> can be executed.</para>
        /// </exception>
        public override void Validate()
        {
            base.Validate();

            if (SourceRevision == null)
                throw new InvalidOperationException("SourceRevision must be set before RebaseGuiCommand can be executed");
            if (DestinationRevision == null)
                throw new InvalidOperationException("DestinationRevision must be set before RebaseGuiCommand can be executed");
        }
    }
}
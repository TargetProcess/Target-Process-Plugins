namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialHookBase"/> descendant implements the
    /// code necessary to handle the "precommit" hook:
    /// This is run before starting a commit (note, the commit command has started executing, and can have addes files, etc.
    /// See the MercurialPreCommitCommandHook for a way to intercept the entire commit command.)
    /// </summary>
    /// <remarks>
    /// As with all controlling hooks (descendants of <see cref="MercurialControllingHookBase"/>), you can
    /// prevent the command from continuing, or let it continue, by calling
    /// <see cref="MercurialControllingHookBase.TerminateHookAndCancelCommand(int)"/>
    /// or <see cref="MercurialControllingHookBase.TerminateHookAndProceed"/> respectively.
    /// </remarks>
    public class MercurialPreCommitHook : MercurialControllingHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="LeftParentRevision"/> property.
        /// </summary>
        private readonly RevSpec _LeftParentRevision = LoadRevision("HG_PARENT1");

        /// <summary>
        /// This is the backing field for the <see cref="RightParentRevision"/> property.
        /// </summary>
        private readonly RevSpec _RightParentRevision = LoadRevision("HG_PARENT2");

        /// <summary>
        /// Gets the <see cref="RevSpec"/> identifying the left parent of the changeset
        /// that was committed.
        /// </summary>
        public RevSpec LeftParentRevision
        {
            get
            {
                return _LeftParentRevision;
            }
        }

        /// <summary>
        /// Gets the <see cref="RevSpec"/> identifying the left parent of the changeset
        /// that was committed; or <c>null</c> if this is not a merge changeset.
        /// </summary>
        public RevSpec RightParentRevision
        {
            get
            {
                return _RightParentRevision;
            }
        }
    }
}

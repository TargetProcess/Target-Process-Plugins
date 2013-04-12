namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialControllingHookBase"/> descendant implements the
    /// code necessary to handle the "preupdate" hook:
    /// This is run before updating the working directory.
    /// </summary>
    /// <remarks>
    /// As with all controlling hooks (descendants of <see cref="MercurialControllingHookBase"/>), you can
    /// prevent the command from continuing, or let it continue, by calling
    /// <see cref="MercurialControllingHookBase.TerminateHookAndCancelCommand(int)"/>
    /// or <see cref="MercurialControllingHookBase.TerminateHookAndProceed"/> respectively.
    /// </remarks>
    public class MercurialPreUpdateHook : MercurialControllingHookBase
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
        /// Gets the <see cref="RevSpec"/> identifying the left parent of the changeset.
        /// </summary>
        public RevSpec LeftParentRevision
        {
            get
            {
                return _LeftParentRevision;
            }
        }

        /// <summary>
        /// Gets the <see cref="RevSpec"/> identifying the right parent of the changeset.
        /// </summary>
        public RevSpec RightParentRevision
        {
            get
            {
                return _RightParentRevision;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current changeset is a merge.
        /// </summary>
        public bool IsMerge
        {
            get
            {
                return _LeftParentRevision != null && _RightParentRevision != null;
            }
        }
    }
}
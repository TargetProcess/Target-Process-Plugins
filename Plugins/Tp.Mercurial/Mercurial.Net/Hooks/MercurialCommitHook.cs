using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialHookBase"/> descendant implements the
    /// code necessary to handle the "commit" hook:
    /// This is run after a new changeset has been created in the local repository.
    /// </summary>
    public class MercurialCommitHook : MercurialHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="CommittedRevision"/> property.
        /// </summary>
        private readonly RevSpec _CommittedRevision = LoadRevision("HG_NODE");

        /// <summary>
        /// This is the backing field for the <see cref="LeftParentRevision"/> property.
        /// </summary>
        private readonly RevSpec _LeftParentRevision = LoadRevision("HG_PARENT1");

        /// <summary>
        /// This is the backing field for the <see cref="RightParentRevision"/> property.
        /// </summary>
        private readonly RevSpec _RightParentRevision = LoadRevision("HG_PARENT2");

        /// <summary>
        /// Gets the <see cref="RevSpec"/> identifying the revision that was committed.
        /// </summary>
        public RevSpec CommittedRevision
        {
            get
            {
                return _CommittedRevision;
            }
        }

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
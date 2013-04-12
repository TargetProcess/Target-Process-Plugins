using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialHookBase"/> descendant implements the
    /// code necessary to handle the "update" hook:
    /// This is run after updating the working directory.
    /// </summary>
    public class MercurialUpdateHook : MercurialHookBase
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
        /// This is the backing field for the <see cref="WasSuccessful"/> property.
        /// </summary>
        private readonly bool _WasSuccessful = (Environment.GetEnvironmentVariable("HG_ERROR") ?? "0") == "0";

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

        /// <summary>
        /// Gets a value indicating whether the update was successful.
        /// </summary>
        public bool WasSuccessful
        {
            get
            {
                return _WasSuccessful;
            }
        }
    }
}
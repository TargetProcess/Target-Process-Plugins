using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialHookBase"/> descendant implements the
    /// code necessary to handle the "tag" hook:
    /// This is run after a tag has been created.
    /// </summary>
    public class MercurialTagHook : MercurialHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="Revision"/> property.
        /// </summary>
        private readonly RevSpec _Revision = LoadRevision("HG_NODE");

        /// <summary>
        /// This is the backing field for the <see cref="IsLocal"/> property.
        /// </summary>
        private readonly bool _IsLocal = (Environment.GetEnvironmentVariable("HG_LOCAL") ?? "0") == "1";

        /// <summary>
        /// This is the backing field for the <see cref="Name"/> property.
        /// </summary>
        private readonly string _Name = Environment.GetEnvironmentVariable("HG_TAG") ?? string.Empty;

        /// <summary>
        /// Gets the <see cref="RevSpec"/> of the changeset that was tagged.
        /// </summary>
        public RevSpec Revision
        {
            get
            {
                return _Revision;
            }
        }

        /// <summary>
        /// Gets the name of the tag that was created.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the tag is a local tag or one that has been
        /// added to the repository.
        /// </summary>
        public bool IsLocal
        {
            get
            {
                return _IsLocal;
            }
        }
    }
}
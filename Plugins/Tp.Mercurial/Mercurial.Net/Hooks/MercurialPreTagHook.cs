using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialControllingHookBase"/> descendant implements the
    /// code necessary to handle the "pretag" hook:
    /// This is run before creating a tag.
    /// </summary>
    /// <remarks>
    /// As with all controlling hooks (descendants of <see cref="MercurialControllingHookBase"/>), you can
    /// prevent the command from continuing, or let it continue, by calling
    /// <see cref="MercurialControllingHookBase.TerminateHookAndCancelCommand(int)"/>
    /// or <see cref="MercurialControllingHookBase.TerminateHookAndProceed"/> respectively.
    /// </remarks>
    public class MercurialPreTagHook : MercurialControllingHookBase
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
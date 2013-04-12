using System;
using System.Diagnostics.CodeAnalysis;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialControllingHookBase"/> descendant implements the
    /// code necessary to handle the "pretxnchangegroup" hook:
    /// This is run after a changegroup has been added via push, pull, or unbundle, but before the transaction has
    /// been committed.
    /// </summary>
    /// <remarks>
    /// As with all controlling hooks (descendants of <see cref="MercurialControllingHookBase"/>), you can
    /// prevent the command from continuing, or let it continue, by calling
    /// <see cref="MercurialControllingHookBase.TerminateHookAndCancelCommand(int)"/>
    /// or <see cref="MercurialControllingHookBase.TerminateHookAndProceed"/> respectively.
    /// </remarks>
    public class MercurialPreTransactionChangegroupHook : MercurialControllingHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="Url"/> property.
        /// </summary>
        private readonly string _Url = Environment.GetEnvironmentVariable("HG_URL") ?? string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="FirstRevision"/> property.
        /// </summary>
        private readonly RevSpec _FirstRevision = LoadRevision("HG_NODE");

        /// <summary>
        /// Gets the source of the incoming changesets.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Cannot guarantee this is a valid Uri, but it is still given as an Url from Mercurial")]
        public string Url
        {
            get
            {
                return _Url;
            }
        }

        /// <summary>
        /// Gets the <see cref="RevSpec"/> of the first changeset.
        /// </summary>
        public RevSpec FirstRevision
        {
            get
            {
                return _FirstRevision;
            }
        }
    }
}
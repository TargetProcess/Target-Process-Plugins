using System;
using System.Diagnostics.CodeAnalysis;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialHookBase"/> descendant implements the
    /// code necessary to handle the "outgoing" hook:
    /// This is run after sending changes from local repository to another.
    /// </summary>
    public class MercurialOutgoingHook : MercurialHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="Source"/> property.
        /// </summary>
        private readonly string _Source = Environment.GetEnvironmentVariable("HG_SOURCE") ?? string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="FirstRevision"/> property.
        /// </summary>
        private readonly RevSpec _FirstRevision = LoadRevision("HG_NODE");

        /// <summary>
        /// This is the backing field for the <see cref="Url"/> property.
        /// </summary>
        private readonly string _Url = Environment.GetEnvironmentVariable("HG_URL") ?? string.Empty;

        /// <summary>
        /// Gets the source of the operation.
        /// </summary>
        public string Source
        {
            get
            {
                return _Source;
            }
        }

        /// <summary>
        /// Gets the <see cref="RevSpec"/> of the first changeset that was sent.
        /// </summary>
        public RevSpec FirstRevision
        {
            get
            {
                return _FirstRevision;
            }
        }

        /// <summary>
        /// Gets the url of the remote repository, if known.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Cannot guarantee this is a valid Uri, but it is still given as an Url from Mercurial")]
        public string Url
        {
            get
            {
                return _Url;
            }
        }
    }
}

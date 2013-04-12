using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialHookBase"/> descendant implements the
    /// code necessary to handle the "incoming" hook:
    /// This is run once for each new changeset that is brought into the repository from elsewhere.
    /// </summary>
    public class MercurialIncomingHook : MercurialHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="Url"/> property.
        /// </summary>
        private readonly string _Url = Environment.GetEnvironmentVariable("HG_URL") ?? string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="Source"/> property.
        /// </summary>
        private readonly string _Source = Environment.GetEnvironmentVariable("HG_SOURCE") ?? string.Empty;

        /// <summary>
        /// This is the backing field for the <see cref="Revision"/> property.
        /// </summary>
        private readonly RevSpec _Revision = LoadRevision("HG_NODE");

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
        /// Gets the <see cref="RevSpec"/> of the changeset that was brought into the repository.
        /// </summary>
        public RevSpec Revision
        {
            get
            {
                return _Revision;
            }
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialControllingHookBase"/> descendant implements the
    /// code necessary to handle the "preoutgoing" hook:
    /// This is run before collecting changes to send from the local repository to another.
    /// </summary>
    /// <remarks>
    /// As with all controlling hooks (descendants of <see cref="MercurialControllingHookBase"/>), you can
    /// prevent the command from continuing, or let it continue, by calling
    /// <see cref="MercurialControllingHookBase.TerminateHookAndCancelCommand(int)"/>
    /// or <see cref="MercurialControllingHookBase.TerminateHookAndProceed"/> respectively.
    /// </remarks>
    public class MercurialPreOutgoingHook : MercurialControllingHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="Source"/> property.
        /// </summary>
        private readonly string _Source = Environment.GetEnvironmentVariable("HG_SOURCE") ?? string.Empty;

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

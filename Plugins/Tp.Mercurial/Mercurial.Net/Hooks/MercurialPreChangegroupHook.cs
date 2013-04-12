using System;
using System.Diagnostics.CodeAnalysis;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialControllingHookBase"/> descendant implements the
    /// code necessary to handle the "prechangegroup" hook:
    /// This is run before a changegroup is added via push, pull, or bundle.
    /// </summary>
    /// <remarks>
    /// As with all controlling hooks (descendants of <see cref="MercurialControllingHookBase"/>), you can
    /// prevent the command from continuing, or let it continue, by calling
    /// <see cref="MercurialControllingHookBase.TerminateHookAndCancelCommand(int)"/>
    /// or <see cref="MercurialControllingHookBase.TerminateHookAndProceed"/> respectively.
    /// </remarks>
    public class MercurialPreChangegroupHook : MercurialControllingHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="Url"/> property.
        /// </summary>
        private readonly string _Url = Environment.GetEnvironmentVariable("HG_URL") ?? string.Empty;

        /// <summary>
        /// Gets the url from which the changes will come in.
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
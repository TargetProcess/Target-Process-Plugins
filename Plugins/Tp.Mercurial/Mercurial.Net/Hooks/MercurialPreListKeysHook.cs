using System;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This <see cref="MercurialControllingHookBase"/> descendant implements the
    /// code necessary to handle the "prelistkeys" hook:
    /// This is run before listing pushkeys (like bookmarks) in the repository.
    /// </summary>
    /// <remarks>
    /// As with all controlling hooks (descendants of <see cref="MercurialControllingHookBase"/>), you can
    /// prevent the command from continuing, or let it continue, by calling
    /// <see cref="MercurialControllingHookBase.TerminateHookAndCancelCommand(int)"/>
    /// or <see cref="MercurialControllingHookBase.TerminateHookAndProceed"/> respectively.
    /// </remarks>
    public class MercurialPreListKeysHook : MercurialControllingHookBase
    {
        /// <summary>
        /// This is the backing field for the <see cref="Namespace"/> property.
        /// </summary>
        private readonly string _Namespace;

        /// <summary>
        /// Initializes a new instance of the <see cref="MercurialPreListKeysHook"/> class.
        /// </summary>
        public MercurialPreListKeysHook()
        {
            _Namespace = Environment.GetEnvironmentVariable("HG_NAMESPACE") ?? string.Empty;
        }

        /// <summary>
        /// Gets the namespace the keys are listed from.
        /// </summary>
        public string Namespace
        {
            get
            {
                return _Namespace;
            }
        }
    }
}
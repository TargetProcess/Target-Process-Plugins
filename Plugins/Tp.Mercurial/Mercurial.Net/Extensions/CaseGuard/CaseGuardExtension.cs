namespace Mercurial.Extensions.CaseGuard
{
    /// <summary>
    /// This class contains logic for the caseguard Mercurial extension.
    /// </summary>
    public static class CaseGuardExtension
    {
        /// <summary>
        /// Gets a value indicating whether the caseguard extension is installed and active.
        /// </summary>
        public static bool IsInstalled
        {
            get
            {
                return ClientExecutable.Configuration.ValueExists("extensions", "caseguard");
            }
        }
    }
}
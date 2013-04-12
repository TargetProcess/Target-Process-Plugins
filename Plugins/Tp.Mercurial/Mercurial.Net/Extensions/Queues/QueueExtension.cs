namespace Mercurial.Extensions.Queues
{
    /// <summary>
    /// This class contains logic for the Mercurial Queues extension.
    /// </summary>
    public static class QueueExtension
    {
        /// <summary>
        /// Gets a value indicating whether the Mercurial Queues (mq) extension is installed and active.
        /// </summary>
        public static bool IsInstalled
        {
            get
            {
                return ClientExecutable.Configuration.ValueExists("extensions", "mq");
            }
        }
    }
}
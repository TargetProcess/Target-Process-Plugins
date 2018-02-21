namespace Tp.Integration.Common
{
    /// <summary>
    /// Describes the type of the Message
    /// </summary>
    public enum CkFinderFileTypeEnum
    {
        /// <summary>
        /// Undefined
        /// </summary>
        None = 0,

        /// <summary>
        /// Inbox
        /// </summary>
        Files = 1,

        /// <summary>
        /// Outbox
        /// </summary>
        Images = 2,

        /// <summary>
        /// Public 
        /// </summary>
        Flash = 3
    }
}

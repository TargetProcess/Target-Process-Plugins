namespace Tp.Integration.Common
{
    /// <summary>
    /// Describes how the request was added
    /// </summary>
    public enum RequestSourceEnum
    {
        /// <summary>
        /// Undefined
        /// </summary>
        None = 0,

        /// <summary>
        /// By email
        /// </summary>
        Email = 1,

        /// <summary>
        /// By phone
        /// </summary>
        Phone = 2,

        /// <summary>
        /// Internally
        /// </summary>
        Internal = 3,

        /// <summary>
        /// Externally
        /// </summary>
        External = 4
    }
}

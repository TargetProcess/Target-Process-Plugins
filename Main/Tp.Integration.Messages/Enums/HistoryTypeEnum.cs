namespace Tp.Integration.Common
{
    public enum HistoryTypeEnum
    {
        /// <summary>
        /// Nothing changes
        /// </summary>
        None = -1,

        /// <summary>
        /// The entity is added
        /// </summary>
        Add = 0,

        /// <summary>
        /// The entity is updated
        /// </summary>
        Update = 1,

        /// <summary>
        /// The entity is deleted
        /// </summary>
        Delete = 2,
    }
}

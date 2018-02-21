namespace Tp.Integration.Common
{
    /// <summary>
    /// Describes what happens with file. The enum is used for Source Control Components
    /// </summary>
    public enum FileActionEnum
    {
        /// <summary>
        /// Undefined
        /// </summary>
        None = 0,

        /// <summary>
        /// The file was added
        /// </summary>
        Add = 1,

        /// <summary>
        /// The file was deleted
        /// </summary>
        Delete = 2,

        /// <summary>
        /// The file was modified
        /// </summary>
        Modify = 3,

        /// <summary>
        /// The file was renamed
        /// </summary>
        Rename = 4,

        /// <summary>
        /// The file was branched
        /// </summary>
        Branch = 5
    }
}

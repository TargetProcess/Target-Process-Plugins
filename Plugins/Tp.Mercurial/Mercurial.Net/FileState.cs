namespace Mercurial
{
    /// <summary>
    /// This enum contains possible file states for files reported by the
    /// <see cref="Repository.Status(StatusCommand)"/> command through the
    /// <see cref="FileStatus"/> object.
    /// </summary>
    public enum FileState
    {
        /// <summary>
        /// The file is not tracked by Mercurial.
        /// </summary>
        /// <remarks>
        /// This corresponds to the status code ? (question mark.)
        /// </remarks>
        Unknown,

        /// <summary>
        /// The file has been modified, but not yet committed.
        /// </summary>
        /// <remarks>
        /// This corresponds to the status code M (upper-case M.)
        /// </remarks>
        Modified,

        /// <summary>
        /// The file has been added, but not yet committed.
        /// </summary>
        /// <remarks>
        /// This corresponds to the status code A (upper-case A.)
        /// </remarks>
        Added,

        /// <summary>
        /// The file has been removed, but not yet committed.
        /// </summary>
        /// <remarks>
        /// This corresponds to the status code R (upper-case R.)
        /// </remarks>
        Removed,

        /// <summary>
        /// The file is clean (tracked, no changes detected.)
        /// </summary>
        /// <remarks>
        /// This corresponds to the status code C (upper-case C.)
        /// </remarks>
        Clean,

        /// <summary>
        /// The file is missing, but not marked as removed.
        /// </summary>
        /// <remarks>
        /// This corresponds to the status code ! (exclamation mark.)
        /// </remarks>
        Missing,

        /// <summary>
        /// The file is ignored.
        /// </summary>
        /// <remarks>
        /// This corresponds to the status code I (upper-case I.)
        /// </remarks>
        Ignored
    }
}
namespace Mercurial
{
    /// <summary>
    /// This enum is used by <see cref="MergeCommand"/> to signal the result of a merge command.
    /// </summary>
    public enum MergeResult
    {
        /// <summary>
        /// The merge was a success, no unresolved files, the working directory is ready to commit.
        /// </summary>
        Success,

        /// <summary>
        /// The merge was a partial success, there are unresolved files that needs to be resolved
        /// before the working directory is ready to commit.
        /// </summary>
        UnresolvedFiles,
    }
}
namespace Mercurial
{
    /// <summary>
    /// This enum is used by <see cref="MergeConflict"/> to signal the current (as of last executionof the
    /// <see cref="ResolveCommand"/>) state of a file that had a merge conflict from the last merge.
    /// </summary>
    public enum MergeConflictState
    {
        /// <summary>
        /// The file is in an unresolved state.
        /// </summary>
        Unresolved,

        /// <summary>
        /// The file is in a resolved state.
        /// </summary>
        Resolved,
    }
}
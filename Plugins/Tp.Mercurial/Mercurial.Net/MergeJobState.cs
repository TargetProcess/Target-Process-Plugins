namespace Mercurial
{
    /// <summary>
    /// This enum is used by a <see cref="MergeJob"/> to indicate its current state.
    /// </summary>
    public enum MergeJobState
    {
        /// <summary>
        /// The <see cref="MergeJob"/> is ready to commit, there are no unresolved conflicts (left.)
        /// </summary>
        ReadyToCommit,

        /// <summary>
        /// The <see cref="MergeJob"/> was cancelled, and reverted back to the left parent of the
        /// merge.
        /// </summary>
        Canceled,

        /// <summary>
        /// The <see cref="MergeJob"/> has been committed.
        /// </summary>
        Committed,

        /// <summary>
        /// The <see cref="MergeJob"/> is still open, and has unresolved conflicts.
        /// </summary>
        HasUnresolvedConflicts
    }
}
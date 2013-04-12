namespace Mercurial
{
    /// <summary>
    /// This enum is used by <see cref="ResolveCommand"/>.<see cref="ResolveCommand.Action">Action</see> to specify
    /// which action to take on the files selected.
    /// </summary>
    public enum ResolveAction
    {
        /// <summary>
        /// Mark the selected files as resolved.
        /// </summary>
        MarkResolved,

        /// <summary>
        /// Mark the selected files as unresolved.
        /// </summary>
        MarkUnresolved,

        /// <summary>
        /// Attempt to redo the merge operation.
        /// </summary>
        RedoMerge,

        /// <summary>
        /// List the resolution status of the selected files.
        /// </summary>
        List,
    }
}
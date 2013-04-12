namespace Mercurial
{
    /// <summary>
    /// This enum is used by <see cref="ChangesetPathAction"/> to specify the
    /// type of action that was performed on the path.
    /// </summary>
    public enum ChangesetPathActionType
    {
        /// <summary>
        /// The path was added in the related changeset.
        /// </summary>
        Add,

        /// <summary>
        /// The path was removed in the related changeset.
        /// </summary>
        Remove,

        /// <summary>
        /// The path was modified in the related changeset.
        /// </summary>
        Modify,
    }
}
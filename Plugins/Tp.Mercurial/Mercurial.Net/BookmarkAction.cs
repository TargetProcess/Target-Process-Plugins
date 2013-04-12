namespace Mercurial
{
    /// <summary>
    /// This enum is used by <see cref="BookmarkCommand"/>.<see cref="BookmarkCommand.Action">Action</see>
    /// to specify which action to take.
    /// </summary>
    public enum BookmarkAction
    {
        /// <summary>
        /// Create a new bookmark.
        /// </summary>
        CreateNew,

        /// <summary>
        /// Move an existing bookmark to a new revision. If the bookmark does not exist, create it
        /// as though <see cref="CreateNew"/> was specified. This is identical to using the "--force"
        /// option with Mercurial.
        /// </summary>
        MoveExisting,

        /// <summary>
        /// Delete an existing bookmark. This is identical to using the "--delete" option with Mercurial.
        /// </summary>
        DeleteExisting,

        /// <summary>
        /// Rename an existing bookmark to a new name. This is identical to using the "--rename NAME"
        /// option with Mercurial.
        /// </summary>
        RenameExisting
    }
}
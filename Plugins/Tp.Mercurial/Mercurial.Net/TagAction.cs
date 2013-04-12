namespace Mercurial
{
    /// <summary>
    /// This enum is used to specify which action to take for a tag, when using the
    /// <see cref="TagCommand"/>.
    /// </summary>
    public enum TagAction
    {
        /// <summary>
        /// Add a new tag with the specified name.
        /// </summary>
        Add,

        /// <summary>
        /// Remove a tag with the specified name.
        /// </summary>
        Remove
    }
}
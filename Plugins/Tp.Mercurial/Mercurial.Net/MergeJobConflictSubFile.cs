namespace Mercurial
{
    /// <summary>
    /// This enum is used by <see cref="MergeJobConflict"/> to get paths to and contents of the
    /// sub-files for a merge conflict.
    /// </summary>
    public enum MergeJobConflictSubFile
    {
        /// <summary>
        /// This is the base file, the common ancestor of both <see cref="Local"/> and <see cref="Other"/>.
        /// </summary>
        Base,

        /// <summary>
        /// The local version of the file, the one that was present in the working folder before the
        /// merge was initiated.
        /// </summary>
        Local,

        /// <summary>
        /// The other version of the file, the one that came from the branch that the merge is attempting
        /// to merge in.
        /// </summary>
        Other,

        /// <summary>
        /// The current file, as present in the working directory. This is the one that will be kept
        /// if the file is marked as resolved.
        /// </summary>
        Current,
    }
}
namespace Mercurial
{
    /// <summary>
    /// This enum is used by the <see cref="BisectCommand"/>.<see cref="BisectCommand.State">State</see>
    /// property, to specify how to continue with the bisect command.
    /// </summary>
    public enum BisectState
    {
        /// <summary>
        /// No state has been set yet, this is the default value and is used to
        /// force the code to set a state before executing the <see cref="BisectCommand"/>.
        /// </summary>
        None,

        /// <summary>
        /// Skip testing the changeset.
        /// </summary>
        Skip,

        /// <summary>
        /// Mark the changeset as good.
        /// </summary>
        Good,

        /// <summary>
        /// Mark the changeset as bad.
        /// </summary>
        Bad,

        /// <summary>
        /// Reset the <see cref="BisectCommand"/>, ending the search for the first good
        /// changeset.
        /// </summary>
        Reset
    }
}
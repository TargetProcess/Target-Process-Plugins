using System;

namespace Mercurial
{
    /// <summary>
    /// This enum is used by the <see cref="DiffCommand"/>.<see cref="DiffCommand.Ignore">Ignore</see> property
    /// to specify whether to ignore certain types of differences that may be insignificant in meaning.
    /// </summary>
    [Flags]
    public enum DiffIgnores
    {
        /// <summary>
        /// Ignore nothing, all changes are meaningful.
        /// </summary>
        None = 0,

        /// <summary>
        /// If this flag is present; ignore all whitespace when comparing lines.
        /// </summary>
        WhiteSpace = 1,

        /// <summary>
        /// If this flag is present; ignore changes in the amount of whitespace.
        /// </summary>
        ChangedWhiteSpace = 2,

        /// <summary>
        /// If this flag is present; ignore changes whose lines are all blank.
        /// </summary>
        BlankLines = 4,

        /// <summary>
        /// Ignore whitespace, changes to whitespace, and blank lines (equivalent to
        /// <see cref="DiffIgnores"/>.<see cref="WhiteSpace"/> | <see cref="DiffIgnores"/>.<see cref="ChangedWhiteSpace"/> | <see cref="DiffIgnores"/>.<see cref="BlankLines"/>.
        /// </summary>
        All = WhiteSpace | ChangedWhiteSpace | BlankLines
    }
}
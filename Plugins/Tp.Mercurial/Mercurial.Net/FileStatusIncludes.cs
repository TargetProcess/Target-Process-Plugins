using System;

namespace Mercurial
{
    /// <summary>
    /// This enum is used by <see cref="StatusCommand.Include"/> to specify what kind of
    /// status codes to include.
    /// </summary>
    [Flags]
    public enum FileStatusIncludes
    {
        /// <summary>
        /// Include only files that have been modified (that has the status code 'M'.)
        /// </summary>
        Modified = 1,

        /// <summary>
        /// Include only files that have been added (that has the status code 'A'.)
        /// </summary>
        Added = 2,

        /// <summary>
        /// Include only files that have been removed (that has the status code 'R'.)
        /// </summary>
        Removed = 4,

        /// <summary>
        /// Include only files that are missing (that has the status code '!', which means the
        /// file is missing on disk, but Mercurial has not been told to remove/forget the file.)
        /// </summary>
        Missing = 8,

        /// <summary>
        /// Include only clean files without changes (that has the status code 'C'.)
        /// </summary>
        Clean = 16,

        /// <summary>
        /// Include only unknown files (that has the status code '?', which means the
        /// file is on disk, but Mercurial has not been told to ignore it nor track it.)
        /// </summary>
        Unknown = 32,

        /// <summary>
        /// Include only ignored files (that has the status code 'I'.)
        /// </summary>
        Ignored = 64,

        /// <summary>
        /// Include all states.
        /// </summary>
        All = Modified | Added | Removed | Missing | Clean | Unknown | Ignored,

        /// <summary>
        /// The default inclusion options, which is everything except ignored and clean (unmodified) files.
        /// </summary>
        Default = Modified | Added | Removed | Missing | Unknown
    }
}
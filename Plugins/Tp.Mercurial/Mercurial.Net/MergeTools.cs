using System;

namespace Mercurial
{
    /// <summary>
    /// This class holds string constants with proper .NET identifiers for the built-in known
    /// merge tools.
    /// </summary>
    public static class MergeTools
    {
        /// <summary>
        /// Uses the internal non-interactive simple merge algorithm for merging files.
        /// It will fail if there are any conflicts and leave markers in the partially merged file.
        /// </summary>
        public const string InternalMerge = "internal:merge";

        /// <summary>
        /// Rather than attempting to merge files that were modified on both branches,
        /// it marks them as unresolved. The <c>resolve</c> command must be used to resolve these conflicts.
        /// </summary>
        public const string InternalFail = "internal:fail";

        /// <summary>
        /// Uses the local version of files as the merged version.
        /// </summary>
        public const string InternalLocal = "internal:local";

        /// <summary>
        /// Uses the other version of files as the merged version.
        /// </summary>
        public const string InternalOther = "internal:other";

        /// <summary>
        /// Asks the user which of the local or the other version to keep as the merged version.
        /// </summary>
        [Obsolete("Do not use this, there is no way to provide feedback to the command client at this time, instead use a MergeJob and control the merge manually that way.")]
        public const string InternalPrompt = "internal:prompt";

        /// <summary>
        /// Creates three versions of the files to merge, containing the contents of local, other and base.
        /// These files can then be used to perform a merge manually.
        /// If the file to be merged is named "a.txt", these files will accordingly be named "a.txt.local",
        /// "a.txt.other" and "a.txt.base" and they will be placed in the same directory as "a.txt".
        /// </summary>
        public const string InternalDump = "internal:dump";
    }
}
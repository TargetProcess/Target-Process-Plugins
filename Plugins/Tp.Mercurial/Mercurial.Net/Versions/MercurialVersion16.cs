using System.Collections.Generic;
using System.Globalization;

namespace Mercurial.Versions
{
    /// <summary>
    /// This <see cref="MercurialVersionBase"/> implements version-specific methods for
    /// Mercurial 1.6.
    /// </summary>
    [MercurialVersion("1.6")]
    public class MercurialVersion16 : MercurialVersionPre18
    {
        /// <summary>
        /// This method produces a collection of options and arguments to pass on the command line
        /// to specify the merge tool.
        /// </summary>
        /// <param name="tool">
        /// The merge tool to generate options and arguments for.
        /// </param>
        /// <returns>
        /// A collection of options and arguments to pass on the command line.
        /// </returns>
        public override IEnumerable<string> MergeToolOption(string tool)
        {
            if (StringEx.IsNullOrWhiteSpace(tool))
                yield break;

            yield return "--config";
            yield return string.Format(CultureInfo.InvariantCulture, "ui.merge={0}", StringEx.EncapsulateInQuotesIfWhitespace(tool));
        }
    }
}
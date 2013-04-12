using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This is the base class for <see cref="MercurialCommandHookDictionary"/>
    /// and <see cref="MercurialCommandHookPatternCollection"/>
    /// </summary>
    public class MercurialCommandHookDataStructureBase
    {
        /// <summary>
        /// This field is used by the parses to return a dummy value that can be compared against.
        /// </summary>
        private static readonly object _DummyValue = new object();

        /// <summary>
        /// Parses the value at the given position and returns it.
        /// </summary>
        /// <param name="optionsCombined">
        /// The string in which to parse a value.
        /// </param>
        /// <param name="index">
        /// The position in <paramref name="optionsCombined"/> to parse a value.
        /// </param>
        /// <returns>
        /// The parsed value, which can be <c>null</c>, a <see cref="String"/>, or an array of strings.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Justification = "Will avoid overly strange return types, keeping the ref")]
        protected object ParseValue(string optionsCombined, ref int index)
        {
            switch (optionsCombined[index])
            {
                case ',':
                case ' ':
                    index++;
                    return _DummyValue;

                case 'N':
                    if (index + 3 < optionsCombined.Length && optionsCombined.Substring(index, 4) == "None")
                        index += 4;
                    return null;

                case '\'':
                    index++;
                    var sb = new StringBuilder();
                    while (optionsCombined[index] != '\'')
                    {
                        if (optionsCombined[index] == '\\')
                        {
                            index++;
                            switch (optionsCombined[index])
                            {
                                case 'n':
                                    sb.Append("\n");
                                    break;

                                case 't':
                                    sb.Append("\t");
                                    break;

                                case 'r':
                                    sb.Append("\r");
                                    break;

                                default:
                                    sb.Append(optionsCombined[index]);
                                    break;
                            }
                        }
                        else
                            sb.Append(optionsCombined[index]);
                        index++;
                    }
                    index++;
                    return sb.ToString();

                case '[':
                    index++;
                    var values = new List<string>();
                    while (optionsCombined[index] != ']')
                    {
                        int prevIndex = index;
                        var value = ParseValue(optionsCombined, ref index);
                        if (prevIndex == index)
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unsupported syntax in options combined at position #{0} (0-based), did not understand value at location", index));
                        values.Add(value as string);

                        if (optionsCombined[index] == ',')
                        {
                            index++;
                            if (optionsCombined[index] == ' ')
                                index++;
                        }
                    }
                    index++;
                    return values.ToArray();

                default:
                    return null;
            }
        }
    }
}

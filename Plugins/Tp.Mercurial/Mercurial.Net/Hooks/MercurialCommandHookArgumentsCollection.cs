using System;
using System.Collections.Generic;
using System.Linq;

namespace Mercurial.Hooks
{
    /// <summary>
    /// This class is used by <see cref="MercurialPreCommandHook"/> and <see cref="MercurialPostCommandHook"/> to
    /// hold the positional arguments to the Mercurial command.
    /// </summary>
    public class MercurialCommandHookArgumentsCollection : IEnumerable<string>
    {
        /// <summary>
        /// This is the backing field for the [int] indexer.
        /// </summary>
        private readonly string[] _Arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="MercurialCommandHookArgumentsCollection"/> class.
        /// </summary>
        public MercurialCommandHookArgumentsCollection()
            : this(Environment.GetEnvironmentVariable("HG_ARGS") ?? string.Empty)
        {
            // Do nothing here
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MercurialCommandHookArgumentsCollection"/> class.
        /// </summary>
        /// <param name="argumentsCombined">
        /// The string containing the arguments to parse.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="argumentsCombined"/> is <c>null</c>.</para>
        /// </exception>
        public MercurialCommandHookArgumentsCollection(string argumentsCombined)
        {
            if (argumentsCombined == null)
                throw new ArgumentNullException("argumentsCombined");

            var arguments = new List<string>();
            int index = 0;

            while (index < argumentsCombined.Length)
            {
                switch (argumentsCombined[index])
                {
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                        index++;
                        break;

                    case '\'':
                    case '"':
                        char quote = argumentsCombined[index];
                        index++;
                        int startIndexInsideQuote = index;
                        while (index < argumentsCombined.Length && argumentsCombined[index] != quote)
                            index++;
                        arguments.Add(argumentsCombined.Substring(startIndexInsideQuote, index - startIndexInsideQuote));
                        index++;
                        break;

                    default:
                        int startIndex = index;
                        while (index < argumentsCombined.Length && argumentsCombined[index] != ' ')
                            index++;
                        arguments.Add(argumentsCombined.Substring(startIndex, index - startIndex));
                        index++;
                        break;
                }
            }

            _Arguments = arguments.Where(s => !StringEx.IsNullOrWhiteSpace(s)).ToArray();
        }

        /// <summary>
        /// Gets the positional argument to the Mercurial command by its index, 0-based.
        /// </summary>
        /// <param name="index">
        /// The index, 0-based, of the positional argument to retrieve.
        /// </param>
        /// <returns>
        /// The positional argument with the given <paramref name="index"/>.
        /// </returns>
        public string this[int index]
        {
            get
            {
                return _Arguments[index];
            }
        }

        /// <summary>
        /// Gets the number of positional arguments to the Mercurial command.
        /// </summary>
        public int Count
        {
            get
            {
                return _Arguments.Length;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return _Arguments.ToList().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

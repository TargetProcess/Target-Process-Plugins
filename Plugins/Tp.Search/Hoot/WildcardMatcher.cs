namespace hOOt
{
    /// <summary>
    /// Types implementing this interface can be used to test whether
    /// string matches given pattern when particular matching rules are applied.
    /// </summary>
    public interface IStringMatcher
    {
        /// <summary>
        /// Pattern against which strings are matched.
        /// </summary>
        string Pattern { get; }
        /// <summary>
        /// Tests whether <paramref name="value"/> is matched by the pattern specified
        /// by the <see cref="Pattern"/> property.
        /// </summary>
        /// <param name="value">Value which is tested for matching with pattern stored in this instance.</param>
        /// <returns>true if <paramref name="value"/> can be matched with <see cref="Pattern"/>; otherwise false.</returns>
        bool IsMatch(string value);
    }

    /// <summary>
    /// Implements string matching algorithm which tries to match pattern containing wildcard characters
    /// with the given input string.
    /// </summary>
    public class WildcardMatcher: IStringMatcher
    {

        /// <summary>
        /// Constructor which initializes pattern against which input strings are matched.
        /// </summary>
        /// <param name="pattern">Pattern used to match input strings.</param>
        public WildcardMatcher(string pattern): this(pattern, DefaultSingleWildcard, DefaultMultipleWildcard)
        {
        }

        /// <summary>
        /// Constructor which initializes pattern against which input strings are matched and
        /// wildcard characters used in string matching.
        /// </summary>
        /// <param name="pattern">Pattern against which input strings are matched.</param>
        /// <param name="singleWildcard">Wildcard character used to replace single character in input strings.</param>
        /// <param name="multipleWildcard">Wildcard character used to replace zero or more consecutive characters in input strings.</param>
        private WildcardMatcher(string pattern, char singleWildcard, char multipleWildcard)
        {
            _pattern = pattern;
            SingleWildcard = singleWildcard;
            MultipleWildcard = multipleWildcard;
            var wildcardChars = new[] { singleWildcard, multipleWildcard };
	         _isStringContainsPatternAction =  !string.IsNullOrEmpty(pattern) && pattern[0] == MultipleWildcard &&
			                                        pattern[pattern.Length - 1] == MultipleWildcard && pattern.Length > 2 &&
																							pattern.IndexOfAny(wildcardChars, 1, pattern.Length - 2) == -1;
        }

        /// <summary>
        /// Gets or sets pattern against which input strings are mapped.
        /// Pattern may contain wildcard characters specified by <see cref="SingleWildcard"/>
        /// and <see cref="MultipleWildcard"/> properties.
        /// Returns empty string if pattern has not been set or it was set to null value.
        /// </summary>
        public string Pattern
        {
            get
            {
                return _pattern;
            }
        }

        /// <summary>
        /// Tries to match <paramref name="value"/> against <see cref="Pattern"/> value stored in this instance.
        /// </summary>
        /// <param name="value">String which should be matched against the contained pattern.</param>
        /// <returns>true if <paramref name="value"/> can be matched with <see cref="Pattern"/>; otherwise false.</returns>
        public bool IsMatch(string value)
        {
            // Check if a string contains pattern for optomization
            if (_isStringContainsPatternAction)
            {
							return value.Contains(Pattern.Substring(1, Pattern.Length - 2));
            }

            var inputPosStack = new int[(value.Length + 1) * (Pattern.Length + 1)];   // Stack containing input positions that should be tested for further matching
            var patternPosStack = new int[inputPosStack.Length];                      // Stack containing pattern positions that should be tested for further matching
            int stackPos = -1;                                                          // Points to last occupied entry in stack; -1 indicates that stack is empty
            var pointTested = new bool[value.Length + 1, Pattern.Length + 1];       // Each true value indicates that input position vs. pattern position has been tested

            int inputPos = 0;   // Position in input matched up to the first multiple wildcard in pattern
            int patternPos = 0; // Position in pattern matched up to the first multiple wildcard in pattern

            // Match beginning of the string until first multiple wildcard in pattern
            while (inputPos < value.Length && patternPos < Pattern.Length &&
                   Pattern[patternPos] != MultipleWildcard &&
                   (value[inputPos] == Pattern[patternPos] || Pattern[patternPos] == SingleWildcard))
            {
                inputPos++;
                patternPos++;
            }

            // Push this position to stack if it points to end of pattern or to a general wildcard character
            if (patternPos == _pattern.Length || _pattern[patternPos] == MultipleWildcard)
            {
                pointTested[inputPos, patternPos] = true;
                inputPosStack[++stackPos] = inputPos;
                patternPosStack[stackPos] = patternPos;
            }

            bool matched = false;

            // Repeat matching until either string is matched against the pattern or no more parts remain on stack to test
            while (stackPos >= 0 && !matched)
            {
                inputPos = inputPosStack[stackPos];         // Pop input and pattern positions from stack
                patternPos = patternPosStack[stackPos--];   // Matching will succeed if rest of the input string matches rest of the pattern

                if (inputPos == value.Length && patternPos == Pattern.Length)
                    matched = true;     // Reached end of both pattern and input string, hence matching is successful
                else if (patternPos == Pattern.Length - 1)
                    matched = true;     // Current pattern character is multiple wildcard and it will match all the remaining characters in the input string
                else
                {
                    // First character in next pattern block is guaranteed to be multiple wildcard
                    // So skip it and search for all matches in value string until next multiple wildcard character is reached in pattern
                    for (int curInputStart = inputPos; curInputStart < value.Length; curInputStart++)
                    {
                        int curInputPos = curInputStart;
                        int curPatternPos = patternPos + 1;

                        while (curInputPos < value.Length && curPatternPos < Pattern.Length &&
                               Pattern[curPatternPos] != MultipleWildcard &&
                               (value[curInputPos] == Pattern[curPatternPos] || Pattern[curPatternPos] == SingleWildcard))
                        {
                            curInputPos++;
                            curPatternPos++;
                        }

                        // If we have reached next multiple wildcard character in pattern without breaking the matching sequence,
                        // then we have another candidate for full match.
                        // This candidate should be pushed to stack for further processing.
                        // At the same time, pair (input position, pattern position) will be marked as tested,
                        // so that it will not be pushed to stack later again.
                        if (((curPatternPos == Pattern.Length && curInputPos == value.Length) ||
                             (curPatternPos < Pattern.Length && Pattern[curPatternPos] == MultipleWildcard)) &&
                            !pointTested[curInputPos, curPatternPos])
                        {
                            pointTested[curInputPos, curPatternPos] = true;
                            inputPosStack[++stackPos] = curInputPos;
                            patternPosStack[stackPos] = curPatternPos;
                        }
                    }
                }
            }
            return matched;
        }

        /// <summary>
        /// Gets or sets wildcard character which is used to replace exactly one character
        /// in the input string (default is question mark - ?).
        /// </summary>
        public char SingleWildcard { get; private set; }

        /// <summary>
        /// Gets or sets wildcard character which is used to replace zero or more characters
        /// in the input string (default is asterisk - *).
        /// </summary>
        public char MultipleWildcard { get; private set; }

				private readonly bool _isStringContainsPatternAction;

        /// <summary>
        /// Pattern against which input strings are matched; may contain wildcard characters.
        /// </summary>
        private readonly string _pattern;

        /// <summary>
        /// Default wildcard character used to map single character from input string.
        /// </summary>
        private const char DefaultSingleWildcard = '?';

        /// <summary>
        /// Default wildcard character used to map zero or more consecutive characters from input string.
        /// </summary>
        private const char DefaultMultipleWildcard = '*';
    }
}

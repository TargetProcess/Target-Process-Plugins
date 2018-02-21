using System;
using System.Collections.Generic;
using System.Linq;

namespace Tp.Core.Diagnostics.Counters
{
    public class CounterKey
    {
        private const char SegmentSeparator = '.';

        public string Value { get; }
        public IReadOnlyList<string> Segments => Value.Split(SegmentSeparator).ToReadOnlyCollection();

        public CounterKey(string segment)
        {
            if (segment.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(segment));
            }

            if (segment.Contains("."))
            {
                throw new ArgumentException(message: $"dot symbol '{SegmentSeparator}' is forbidden in segments", paramName: nameof(segment));
            }

            Value = segment;
        }

        private CounterKey(CounterKey left, CounterKey right)
        {
            Value = $"{left.Value}{SegmentSeparator}{right.Value}";
        }

        public bool StartsWith(CounterKey otherkey)
        {
            return Value.StartsWith(otherkey.Value);
        }

        public static CounterKey JoinEscaping(CounterKey counterKey, string otherSegment)
        {
            var preparedOther = otherSegment?.Replace(SegmentSeparator, '_');
            return new CounterKey(counterKey, new CounterKey(preparedOther));
        }
    }

    public static class CounterKeyExtensions
    {
        public static CounterKey JoinEscaping(this CounterKey counterKey, string otherSegment)
        {
            return CounterKey.JoinEscaping(counterKey, otherSegment);
        }
    }
}

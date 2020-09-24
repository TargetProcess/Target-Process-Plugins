using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tp.Core
{
    public class Delta
    {
        private static readonly ReadOnlyCollection<string> EmptyChangedFields = new ReadOnlyCollection<string>(Array.Empty<string>());

        protected Delta(object original, object changed, IReadOnlyList<string> changedFields)
        {
            Original = original;
            Changed = changed;
            ChangedFields = changedFields;
        }

        public object Original { get; }

        public object Changed { get; }

        public IReadOnlyList<string> ChangedFields { get; }

        public override string ToString()
        {
            return "Delta. Original: {0}, Changed: {1}".Fmt(Original, Changed);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Delta);
        }

        private bool Equals(Delta other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(Original, other.Original) && Equals(Changed, other.Changed);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Original?.GetHashCode() ?? 0) * 397) ^ (Changed?.GetHashCode() ?? 0);
            }
        }

        public static Delta<T> Create<T>(T original, T changed, IReadOnlyList<string> changedFields = null)
        {
            return new Delta<T>(original, changed, changedFields ?? EmptyChangedFields);
        }
    }

    public class Delta<TData> : Delta
    {
        public Delta(TData original, TData changed, IReadOnlyList<string> changedFields) : base(original, changed, changedFields)
        {
        }

        public new TData Original => (TData) base.Original;

        public new TData Changed => (TData) base.Changed;

        public Delta<TData> WithNewChangedFields(IReadOnlyList<string> changedFields) =>
            new Delta<TData>(Original, Changed, changedFields);

        public Delta<TData> WithAppendedChangedFields(IEnumerable<string> changedFields) =>
            WithNewChangedFields(ChangedFields.Concat(changedFields).Distinct().ToList());

        public override bool Equals(object obj)
        {
            return Equals(obj as Delta<TData>);
        }

        private bool Equals(Delta<TData> other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(Changed, other.Changed);
        }

        public override int GetHashCode()
        {
            return Changed != null ? Changed.GetHashCode() : 0;
        }
    }

    public static class DeltaExtensions
    {
        public static string Dump(this IEnumerable<Delta> deltas) =>
            string.Join(Environment.NewLine, deltas.Select(x => x.ToString()));

        public static Delta<TResult> Select<T, TResult>(this Delta<T> delta, Func<T, TResult> map) =>
            new Delta<TResult>(map(delta.Original), map(delta.Changed), delta.ChangedFields);

        public static Delta<TResult> Select<TResult>(this Delta delta, Func<object, TResult> map) =>
            new Delta<TResult>(map(delta.Original), map(delta.Changed), delta.ChangedFields);

        public static T GetChangedOrOriginal<T>(this Delta<Maybe<T>> delta) =>
            delta.Changed.OrElse(() => delta.Original).Value;
    }
}

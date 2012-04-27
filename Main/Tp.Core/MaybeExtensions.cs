using System;
using System.Collections.Generic;
using System.Linq;

namespace Tp.Core
{
	public static class MaybeExtensions
	{
		public static Maybe<TTo> Bind<TTo, TFrom>(this Maybe<TFrom> m, Func<TFrom, Maybe<TTo>> f)
		{
			return Maybe.Bind(m, f);
		}
		
		public static Maybe<TTo> Convert<TTo, TFrom>(this Maybe<TFrom> m, Func<TFrom, TTo> converter)
		{
			return m.Bind(x => Maybe.Just(converter(x)));
		}

		public static Maybe<T> RunUntilFirstSuccess<T>(this IEnumerable<Func<Maybe<T>>> fs)
		{
			foreach (Func<Maybe<T>> f in fs)
			{
				Maybe<T> maybe = f();
				if (maybe.HasValue)
				{
					return maybe;
				}
			}
			return Maybe.Nothing;
		}

		public static Maybe<T> FirstOrDefault<T>(this IEnumerable<Maybe<T>> enumerable)
		{
			foreach (var classifier in enumerable)
			{
				Maybe<T> maybe = classifier;
				if (maybe.HasValue)
				{
					return maybe;
				}
			}
			return Maybe.Nothing;
		}

		public static T ChooseFirst<T>(this IEnumerable<Maybe<T>> items, T defaultObj)
		{
			return Choose(items).FirstOrDefault(defaultObj);
		}

		public static T ChooseFirst<T>(this IEnumerable<Maybe<T>> items)
		{
			return Choose(items).First();
		}

		public static IEnumerable<T> Choose<T>(IEnumerable<Maybe<T>> items)
		{
			return items.Where(i => i.HasValue).Select(i => i.Value);
		}
	}
}
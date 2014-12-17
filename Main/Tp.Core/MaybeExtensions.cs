//
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Tp.Core.Annotations;

namespace Tp.Core
{
	public static class MaybeExtensions
	{
		[DebuggerStepThrough]
		public static void Do<TFrom>(this Maybe<TFrom> m, Action<TFrom> f, Action @else = null)
		{
			if (m.HasValue)
			{
				f(m.Value);
			}
			else
			{
				if (@else != null)
					@else();
			}
		}

		[DebuggerStepThrough]
		public static bool TryGetValue<T>(this Maybe<T> m, out T value)
		{
			if (m.HasValue)
			{
				value = m.Value;
				return true;
			}

			value = default(T);
			return false;
		}

		[DebuggerStepThrough]
		public static Maybe<TTo> Bind<TFrom, TTo>(this Maybe<TFrom> m, [InstantHandle]Func<TFrom, TTo> f)
		{
			return m.HasValue? Maybe.Return(f(m.Value)) : Maybe<TTo>.Nothing;
		}

		[DebuggerStepThrough]
		public static Maybe<T> Where<T>(this Maybe<T> m, Func<T, bool> condition)
		{
			return m.HasValue && condition(m.Value) ? m : Maybe<T>.Nothing;
		}

		[DebuggerStepThrough]
		public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> items)
		{
			return FirstOrNothing(items, x => true);
		}

		[DebuggerStepThrough]
		public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> items)
		{
			return SingleOrNothing(items, x => true);
		}

		[DebuggerStepThrough]
		public static Maybe<T> FirstOrNothing<T>(this IEnumerable<T> items, [InstantHandle] Func<T, bool> condition)
		{
			using (var enumerator = items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					var current = enumerator.Current;
					if (condition(current))
						return Maybe.Just(current);
				}
				return Maybe.Nothing;
			}
		}

		[DebuggerStepThrough]
		public static Maybe<T> SingleOrNothing<T>(this IEnumerable<T> items, Func<T, bool> condition)
		{
			var result = Maybe<T>.Nothing;
			using (var enumerator = items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					var current = enumerator.Current;
					if (condition(current))
					{
						if (result.HasValue)
						{
							return Maybe.Nothing;
						}
						result = Maybe.Just(current);
					}
				}
				return result;
			}
		}

		[DebuggerStepThrough]
		public static IEnumerable<Maybe<TTo>> Bind<TTo, TFrom>(this IEnumerable<Maybe<TFrom>> m, Func<TFrom, Maybe<TTo>> f)
		{
			return m.Select(x => x.Select(f));
		}

		[DebuggerStepThrough]
		public static Maybe<TTo> Select<TFrom, TTo>(this Maybe<TFrom> m, [InstantHandle]Func<TFrom, Maybe<TTo>> f)
		{
			return m.HasValue ? f(m.Value) : Maybe.Nothing;
		}

		[DebuggerStepThrough]
		public static Try<TVal> ToTry<TVal, TError>(this Maybe<TVal> maybe, [InstantHandle]Func<TError> error) where TError : Exception
		{
			if (maybe.HasValue)
			{
				return new Success<TVal>(maybe.Value);
			}
			return new Failure<TVal>(error());
		}

		[DebuggerStepThrough]
		public static TVal GetOrThrow<TVal, TError>(this Maybe<TVal> maybe, [InstantHandle]Func<TError> error) where TError : Exception
		{
			if (!maybe.HasValue)
			{
				throw error();
			}
			return maybe.Value;
		}

		[DebuggerStepThrough]
		public static Maybe<T> RunUntilFirstSuccess<T>(this IEnumerable<Func<Maybe<T>>> tries)
		{
			return tries.Select(f => f()).Where(m => m.HasValue).FirstOrDefault(Maybe.Nothing);
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> Choose<T>(this IEnumerable<Maybe<T>> items)
		{
			return items.Choose(x => x);
		}

		public static Maybe<IEnumerable<T>> Sequence<T>(this IEnumerable<Maybe<T>> parts)
		{
			var result = new List<T>();
			foreach (var maybe in parts)
			{
				if (maybe.HasValue)
				{
					result.Add(maybe.Value);
				}
				else
				{
					return Maybe.Nothing;
				}
			}
			return Maybe.Just((IEnumerable<T>)result.AsReadOnly());
		}



		[DebuggerStepThrough]
		public static IEnumerable<TResult> Choose<T, TResult>(this IEnumerable<Maybe<T>> items, Func<T, TResult> f)
		{
			return items.Where(i => i.HasValue).Select(i => f(i.Value));
		}

		[DebuggerStepThrough]
		public static IObservable<T> Choose<T>(this IObservable<Maybe<T>> items)
		{
			return items.Where(i => i.HasValue).Select(i => i.Value);
		}

		[DebuggerStepThrough]
		public static Maybe<TC> SelectMany<TA, TB, TC>(this Maybe<TA> ma, Func<TA, Maybe<TB>> func, Func<TA, TB, TC> selector)
		{
			return ma.Select(a => func(a).Select(b => Maybe.Just(selector(a, b))));
		}

		[DebuggerStepThrough]
		public static Maybe<TTo> MaybeAs<TTo>(this object o, bool nullMeansNothing = true)
		{
			// ReSharper disable ExpressionIsAlwaysNull
			if (!nullMeansNothing && o == null)
				return Maybe.Just((TTo)o);
			// ReSharper restore ExpressionIsAlwaysNull

			return Maybe.Cast<object, TTo>(o);
		}

		[DebuggerStepThrough]
		public static Maybe<IEnumerable<T>> NothingIfEmpty<T>(this IEnumerable<T> xs) where T : class
		{
			return xs.Any() ? Maybe.Return(xs) : Maybe.Nothing;
		}

		[DebuggerStepThrough]
		public static Maybe<T> NothingIfNull<T>(this T o) where T : class
		{
			return Maybe.ReturnIfNotNull(o);
		}

		[DebuggerStepThrough]
		public static Maybe<T> NothingIfNull<T>(this T? o) where T : struct
		{
			return o.HasValue ? Maybe.Just(o.Value) : Maybe.Nothing;
		}

		[DebuggerStepThrough]
		public static Maybe<T> OrElse<T>(this Maybe<T> maybe, [InstantHandle] Func<Maybe<T>> @else)
		{
			return maybe == Maybe.Nothing ? @else() : maybe;
		}

		[DebuggerStepThrough]
		public static T GetOrElse<T>(this Maybe<T> maybe, [InstantHandle] Func<T> @else)
		{
			return maybe == Maybe.Nothing ? @else() : maybe.Value;
		}

		[DebuggerStepThrough]
		public static T GetOrDefault<T>(this Maybe<T> maybe, T @default = default(T))
		{
			return maybe == Maybe.Nothing ? @default : maybe.Value;
		}

		[DebuggerStepThrough]
		public static Maybe<TResult> Either<T1, T2, TResult>(
			this Maybe<T1> left, Maybe<T2> right,
			[InstantHandle] Func<T1, TResult> caseLeft,
			[InstantHandle] Func<T2, TResult> caseRight)
		{
			if (left.HasValue)
			{
				return Maybe.Just(caseLeft(left.Value));
			}

			if (right.HasValue)
			{
				return Maybe.Just(caseRight(right.Value));
			}

			return Maybe.Nothing;
		}

		public static T? ToNullable<T>(this Maybe<T> maybe) where T : struct
		{
			return maybe.HasValue ? maybe.Value : (T?) null;
		}

		[DebuggerStepThrough]
		public static IEnumerable<T> ToEnumerable<T>(this Maybe<T> maybe)
		{
			if (maybe.HasValue)
			{
				yield return maybe.Value;
			}
		}

	}
}

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Tp.Testing.Common.NUnit
{
	/// <summary>
	/// The Be class is a synonym for Is intended for use with the Should extension methods for more DSL-like syntax
	/// </summary>
	public class Be : Is
	{
		public static CollectionEquivalentConstraint EquivalentTo<T>(IEnumerable<T> expected)
		{
			var array = expected as T[] ?? expected.ToArray();

			return new CollectionEquivalentConstraint(array);
		}

		public static CollectionItemsEqualConstraint EquivalentTo<T>(IEnumerable<T> expected, IEqualityComparer<T> comparer)
		{
			var array = expected as T[] ?? expected.ToArray();

			return new CollectionEquivalentConstraint(array).Using(comparer);
		}

		public static CollectionContainsConstraint Containing<T>(T expected)
		{
			return new CollectionContainsConstraint(expected);
		}

		public static JsonEqualConstraint JsonEqual(object expected)
		{
			return new JsonEqualConstraint(expected);
		}

		public static Constraint Nothing => new NothingConstraint();
	}
}

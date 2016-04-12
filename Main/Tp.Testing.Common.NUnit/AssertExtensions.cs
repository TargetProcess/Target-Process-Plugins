using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Tp.Testing.Common.NUnit
{
	/// <summary>
	/// Extension methods to make working with NUnit in C# 3 a little more DSL-like
	/// </summary>
	public static class AssertExtensions
	{
		/// <summary>
		/// Apply a constraint to an actual value, succeeding if the constraint is satisfied and throwing an assertion exception on failure.
		/// </summary>
		/// <param name="actual">The actual value to test</param>
		/// <param name="constraint">A Constraint to be applied</param>
		/// <param name="message">The message that will be displayed on failure</param>
		/// <param name="args">Arguments to be used in formatting the message</param>
		[DebuggerStepThrough]
		public static void Should(this object actual, IResolveConstraint constraint, string message, params object[] args)
		{
			if (message == null) { throw new ArgumentNullException(nameof(message)); }

			Assert.That(actual, constraint, message, args);
		}

		[DebuggerStepThrough]
		public static void Should<T>(this IEnumerable<T> actual, CollectionEquivalentConstraint constraint, string message, params object[] args)
		{
			if (message == null) { throw new ArgumentNullException(nameof(message)); }

			if (!actual.GetType().IsArray)
				actual = actual.ToArray();

			Assert.That(actual, constraint, message, args);
		}
	}
}

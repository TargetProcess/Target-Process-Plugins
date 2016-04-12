using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tp.Core.Expressions.Visitors
{
	// replace `a==b` with `Math.Abs(a-b) < e`
	class FixFloatComparisonVisitor : ExpressionVisitor
	{
		private readonly float _deltaFloat;
		private static readonly Dictionary<Tuple<Type, Type>, MethodInfo> _compareMethods;

		// ReSharper disable UnusedMember.Local
		private bool Compare(float a, float b)
		{
			return Math.Abs(a - b) < _deltaFloat;
		}

		private bool Compare(float? a, float b)
		{
			return a.HasValue && Compare(a.Value, b);
		}

		private bool Compare(float a, float? b)
		{
			return b.HasValue && Compare(a, b.Value);
		}

		private bool Compare(float? a, float? b)
		{
			if (a.HasValue && b.HasValue)
			{
				return Compare(a.Value, b.Value);
			}
			return !a.HasValue && !b.HasValue;
		}

		private bool Compare(double a, double b)
		{
			return Math.Abs(a - b) < _deltaFloat;
		}

		private bool Compare(double? a, double b)
		{
			return a.HasValue && Compare(a.Value, b);
		}

		private bool Compare(double a, double? b)
		{
			return b.HasValue && Compare(a, b.Value);
		}


		private bool Compare(double? a, double? b)
		{
			if (a.HasValue && b.HasValue)
			{
				return Compare(a.Value, b.Value);
			}
			return !a.HasValue && !b.HasValue;
		}


		private bool Compare(decimal a, decimal b)
		{
			return Math.Abs(a - b) < (decimal) _deltaFloat;
		}

		private bool Compare(decimal? a, decimal b)
		{
			return a.HasValue && Compare(a.Value, b);
		}

		private bool Compare(decimal a, decimal? b)
		{
			return b.HasValue && Compare(a, b.Value);
		}

		private bool Compare(decimal? a, decimal? b)
		{
			if (a.HasValue && b.HasValue)
			{
				return Compare(a.Value, b.Value);
			}
			return !a.HasValue && !b.HasValue;
		}

		// ReSharper restore UnusedMember.Local

		static FixFloatComparisonVisitor()
		{
			_compareMethods =
				typeof(FixFloatComparisonVisitor).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
					.Where(x => x.Name == "Compare")
					.ToDictionary(x =>
					{
						var patameters = x.GetParameters();
						return Tuple.Create(patameters[0].ParameterType, patameters[1].ParameterType);
					});
		}

		public FixFloatComparisonVisitor(float delta)
		{
			_deltaFloat = delta;
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node.NodeType == ExpressionType.Equal)
			{
				var compareMethod = _compareMethods.GetValue(Tuple.Create(node.Left.Type, node.Right.Type));
				return compareMethod.Select(x =>
				{
					Expression expression = Expression.Call(Expression.Constant(this), x, node.Left, node.Right);
					return expression;
				})
					.GetOrDefault(node);
			}
			return node;
		}

		protected override Expression VisitExtension(Expression node)
		{
			return node;
		}
	}
}

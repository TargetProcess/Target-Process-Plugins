using NUnit.Framework.Constraints;

namespace Tp.Testing.Common.NUnit
{
	public static class ConstraintExpressionExtensions
	{
		public static NothingConstraint Nothing(this ConstraintExpression expression)
		{
			return (NothingConstraint) expression.Append(new NothingConstraint());
		}

		public static ResolvableConstraintExpression Value(this ConstraintExpression expression)
		{
			return expression.Append(new ValueOperator());
		}

		public static Constraint Containing<T>(this ConstraintExpression constraintExpression, T expected)
		{
			return constraintExpression.Append(new CollectionContainsConstraint(expected));
		}
	}
}

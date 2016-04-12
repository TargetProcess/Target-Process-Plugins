using NUnit.Framework.Constraints;
using Tp.Core;

namespace Tp.Testing.Common.NUnit
{
	public class ValueConstraint : Constraint
	{
		private readonly Constraint _valueConstraint;

		public ValueConstraint(Constraint valueConstraint)
		{
			_valueConstraint = valueConstraint;
		}

		public override bool Matches(object actual)
		{
			this.actual = actual;
			var maybe = actual as IMaybe;

			if (maybe?.HasValue != true)
				return false;

			return _valueConstraint.Matches(maybe.Value);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("just value");


			if (_valueConstraint is EqualConstraint)
				writer.WritePredicate("equal to");

			_valueConstraint.WriteDescriptionTo(writer);
		}

		protected override string GetStringRepresentation()
		{
			return $"<just {_valueConstraint}>";
		}
	}
}

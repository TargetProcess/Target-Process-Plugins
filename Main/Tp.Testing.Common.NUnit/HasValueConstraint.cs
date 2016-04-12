using NUnit.Framework.Constraints;
using Tp.Core;

namespace Tp.Testing.Common.NUnit
{
	public class HasValueConstraint : Constraint
	{
		public override bool Matches(object actual)
		{
			this.actual = actual;
			var maybe = actual as IMaybe;
			return maybe?.HasValue == true;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WriteExpectedValue("some value");
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			writer.WriteActualValue(actual);
		}

		protected override string GetStringRepresentation()
		{
			return "<some value>";
		}
	}
}

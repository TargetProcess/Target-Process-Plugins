using NUnit.Framework.Constraints;
using Tp.Core;

namespace Tp.Testing.Common.NUnit
{
    public class NothingConstraint : Constraint
    {
        public override bool Matches(object actualValue)
        {
            actual = actualValue;
            var maybe = actualValue as IMaybe;
            return maybe?.HasValue == false;
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.WriteExpectedValue("<nothing>");
        }

        protected override string GetStringRepresentation()
        {
            return "<nothing>";
        }
    }
}

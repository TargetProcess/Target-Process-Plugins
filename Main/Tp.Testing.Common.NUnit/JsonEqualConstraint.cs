using Newtonsoft.Json;
using NUnit.Framework.Constraints;

namespace Tp.Testing.Common.NUnit
{
    public class JsonEqualConstraint : EqualConstraint
    {
        public JsonEqualConstraint(object expected) : base(JsonConvert.SerializeObject(expected))
        {
        }

        public override bool Matches(object actualValue)
        {
            return base.Matches(JsonConvert.SerializeObject(actualValue));
        }
    }
}

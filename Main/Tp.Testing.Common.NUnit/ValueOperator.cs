using NUnit.Framework.Constraints;

namespace Tp.Testing.Common.NUnit
{
    public class ValueOperator : SelfResolvingOperator
    {
        public ValueOperator()
        {
            left_precedence = right_precedence = 1;
        }

        public override void Reduce(ConstraintBuilder.ConstraintStack stack)
        {
            if (RightContext == null || RightContext is BinaryOperator)
                stack.Push(new HasValueConstraint());
            else
                stack.Push(new ValueConstraint(stack.Pop()));
        }
    }
}

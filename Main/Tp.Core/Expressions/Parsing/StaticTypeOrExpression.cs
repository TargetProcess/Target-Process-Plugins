using System;
using System.Linq.Expressions;
using Tp.Core.Annotations;

namespace Tp.Core.Expressions.Parsing
{
    public class StaticTypeOrExpression
    {
        public StaticTypeOrExpression(Type type)
        {
            Expression = null;
            Type = type;
            IsStatic = true;
        }

        public StaticTypeOrExpression(Expression expression)
        {
            Expression = expression;
            Type = expression.Type;
            IsStatic = false;
        }

        public bool IsStatic { get; }

        [NotNull]
        public Type Type { get; }

        [CanBeNull]
        public Expression Expression { get; }

        public override string ToString() => $"{(IsStatic ? "Static" : string.Empty)} {Type.Name}";
    }
}

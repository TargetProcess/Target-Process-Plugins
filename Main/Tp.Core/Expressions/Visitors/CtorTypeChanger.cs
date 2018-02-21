using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tp.Core.Expressions.Visitors
{
    /// <summary>
    /// Replaces constructions of one type with constructions of another one. 
    /// Supports generic types.
    /// Matches properties by name in case types has properties with the same name.
    /// </summary>
    class CtorTypeChanger : ExpressionVisitor
    {
        private readonly Type _baseType;
        private readonly Type _derivedType;

        public CtorTypeChanger(Type baseType, Type derivedType)
        {
            _baseType = baseType;
            _derivedType = derivedType;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            if (node.Type == _baseType)
            {
                return ReplaceConstructor(node, _derivedType);
            }
            if (node.Type.IsConstructedGenericType && _baseType.IsGenericType && node.Type.GetGenericTypeDefinition() == _baseType)
            {
                return ReplaceConstructor(node, _derivedType.MakeGenericType(node.Type.GenericTypeArguments));
            }
            return base.VisitNew(node);
        }

        protected override Expression VisitMemberInit(MemberInitExpression expression)
        {
            var newExpression = (NewExpression) Visit(expression.NewExpression);
            if (newExpression.Type == expression.NewExpression.Type)
            {
                return base.VisitMemberInit(expression);
            }

            var bindings = expression.Bindings.Select(b =>
            {
                var memberAssignment = b as MemberAssignment;
                if (memberAssignment == null)
                {
                    return b;
                }
                var member = newExpression.Type.GetMember(memberAssignment.Member.Name).Single();
                if (member.DeclaringType == memberAssignment.Member.DeclaringType)
                {
                    return b;
                }
                return (MemberBinding) Expression.Bind(member, memberAssignment.Expression);
            });
            return Expression.MemberInit(newExpression, bindings);
        }

        private Expression CreateNewConstructor(NewExpression node, ConstructorInfo constructor)
        {
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (node.Members != null)
            {
                return Expression.New(constructor, node.Arguments, node.Members);
            }
            else
            {
                return Expression.New(constructor, node.Arguments);
            }
        }

        private Expression ReplaceConstructor(NewExpression node, Type type)
        {
            var constructor = FindApropriateCtor(node.Constructor, type);
            return CreateNewConstructor(node, constructor);
        }

        private ConstructorInfo FindApropriateCtor(ConstructorInfo constructor, Type type)
        {
            return type.GetConstructor(constructor.GetParameters().Select(x => x.ParameterType).ToArray());
        }
    }

    class CtorTypeChanger<TBase, TDerived> : CtorTypeChanger
    {
        public CtorTypeChanger() : base(typeof(TBase), typeof(TDerived))
        {
        }
    }
}

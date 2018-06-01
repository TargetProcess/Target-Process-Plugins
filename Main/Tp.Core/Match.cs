using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Tp.Core.Annotations;
using Tp.Core.Expressions;

namespace Tp.Core
{
    public static class PatternMatching
    {
        [NotNull]
        public static CaseClause<T, object> Match<T>(this T o)
        {
            return new CaseClause<T, object>(o);
        }

        [NotNull]
        public static CaseClause<TSource, TResult> Match<TSource, TResult>(this TSource o, TResult typeMarker)
        {
            return new CaseClause<TSource, TResult>(o);
        }

        [NotNull]
        public static T InlineMatch<T>(this T e) where T : Expression
        {
            var visitor = new ReplaceVisitor();
            return (T) visitor.Visit(e);
        }        

        [NotNull]
        public static IQueryable<T> InlineMatch<T>(this IQueryable<T> e)
        {
            return e.TransformExpression(InlineMatch);
        }

        public class CaseClause<T, TResult>
        {
            private Maybe<TResult> _result;
            private readonly T _root;

            internal CaseClause(T root)
            {
                _root = root;
                _result = Maybe.Nothing;
            }

            [NotNull]
            public CaseClause<T, TResult> Case(Func<T, bool> @case, TResult result)
            {
                if (!_result.HasValue && @case(_root))
                    _result = Maybe.Just(result);
                return this;
            }

            [NotNull]
            public CaseClause<T, TResult> Case(Func<T, bool> @case, Func<T, TResult> func)
            {
                if (!_result.HasValue && @case(_root))
                    _result = Maybe.Just(func(_root));
                return this;
            }

            [NotNull]
            public CaseClause<T, TResult> Case<T1>([InstantHandle] Func<T1, TResult> func)
            {
                return Case(x => x is T1, x => func((T1) (object) x));
            }

            [NotNull]
            public CaseClause<T, TResult> Case<T1>(Func<T1, bool> predicate, Func<T1, TResult> func)
            {
                return Case(x => x is T1 && predicate((T1) (object) x), x => func((T1) (object) x));
            }

            [NotNull]
            public TResult End([InstantHandle] Func<T, TResult> @default)
            {
                if (@default == null)
                    @default = _ => default;
                var r = End();
                return r.HasValue ? r.Value : @default(_root);
            }

            public Maybe<TResult> End()
            {
                return _result;
            }

            /// <summary>
            /// Throws if no Case matched imput
            /// </summary>
            [NotNull]
            public TResult EndExhaustive()
            {
                return End(_ => { throw new ArgumentException("No Case clause matched input"); });
            }

            public TResult End(TResult @default)
            {
                return End().GetOrDefault(@default);
            }
        }

        internal sealed class ReplaceVisitor : ExpressionVisitor
        {
            private Expression _argument;
            public Expression _elseExpression;

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                return node.Method
                    .Match(default(Expression))
                    .Case(IsEndMethod, _ => VisitEndMethod(node))
                    .Case(IsCaseWithTypeMethod, _ => VisitForWithType(node))
                    .Case(IsCaseMethod, _ => VisitForWithCondition(node))
                    .End(_ => base.VisitMethodCall(node));
            }

            private bool IsCaseWithTypeMethod(MethodInfo method)
            {
                return IsCaseMethod(method) && method.IsGenericMethod;
            }

            private Expression VisitEndMethod(MethodCallExpression node)
            {
                MethodCallExpression matchCall =
                    ExpressionEnumerable.All(node.Object).OfType<MethodCallExpression>().First(
                        x => x.NodeType == ExpressionType.Call && IsMatchMethod(x.Method));

                _argument = matchCall.Arguments[0];

                if (node.Arguments.Count == 0)
                {
                    throw new NotSupportedException("Could not inline Match expression without default End value");
                }

                var argument = node.Arguments[0];

                Expression newBody = argument.MaybeAs<LambdaExpression>()
                    .Select(x => x.ApplyLambdaParameterExpressions(_argument))
                    .GetOrDefault(argument);

                _elseExpression = newBody;

                base.VisitMethodCall(node);
                return _elseExpression;
            }

            private Expression VisitForWithType(MethodCallExpression node)
            {
                var toType = node.Method.GetGenericArguments()[0];
                if (_argument.Type.IsAssignableFrom(toType) || toType.IsAssignableFrom(_argument.Type))
                {
                    var conditionBody = Expression.TypeIs(_argument, toType);
                    var casted = Expression.Convert(_argument, toType);

                    var lambda = node.Arguments[0] as LambdaExpression;

                    var funcBody = lambda.ApplyLambdaParameterExpressions(casted);

                    _elseExpression = Expression.Condition(conditionBody, funcBody, _elseExpression);
                }
                return base.VisitMethodCall(node);
            }

            private Expression VisitForWithCondition(MethodCallExpression node)
            {
                var cond = node.Arguments[0] as LambdaExpression;
                var result = node.Arguments[1];
                if (result is LambdaExpression)
                {
                    var endLambda = (LambdaExpression) result;
                    result = endLambda.ApplyLambdaParameterExpressions(_argument);
                }

                var newCondition = cond.ApplyLambdaParameterExpressions(_argument);

                _elseExpression = Expression.Condition(newCondition, result, _elseExpression);

                return base.VisitMethodCall(node);
            }

            private bool IsCaseMethod(MethodInfo method)
            {
                return IsMethodFromCase(method) && method.Name == "Case";
            }

            private bool IsMatchMethod(MethodInfo method)
            {
                if (method.IsGenericMethod)
                {
                    return method.DeclaringType == typeof(PatternMatching) && method.Name == "Match";
                }
                return false;
            }

            private bool IsEndMethod(MethodInfo method)
            {
                return IsMethodFromCase(method) && method.Name == "End";
            }

            private static bool IsMethodFromCase(MethodInfo method)
            {
                return method.DeclaringType.IsGenericType
                    && method.DeclaringType.GetGenericTypeDefinition() == typeof(CaseClause<object, object>).GetGenericTypeDefinition();
            }

            protected override Expression VisitExtension(Expression node)
            {
                return node;
            }
        }

        public static TypedCaseE<TSource, TResult> MatchE<TSource, TResult>()
        {
            return new TypedCaseE<TSource, TResult>();
        }

        public static CaseE<LambdaExpression, LambdaExpression> MatchE()
        {
            return new CaseE<LambdaExpression, LambdaExpression>();
        }

        private static readonly MethodInfo MatchMethodInfo =
            Reflect.GetMethod<object>(x => x.Match(default(object))).GetGenericMethodDefinition();

        public class CaseE<TCondition, TResult>
            where TCondition : LambdaExpression
            where TResult : LambdaExpression
        {
            protected readonly CaseE<TCondition, TResult> PreviousCase;
            public TCondition Condition { get; }
            public TResult Result { get; }

            public CaseE(CaseE<TCondition, TResult> previousCase, TCondition condition, TResult result)
            {
                PreviousCase = previousCase;
                Condition = condition;
                Result = result;
            }

            public CaseE()
            {
            }

            public CaseE<TCondition, TResult> Case(TCondition @case, TResult then)
            {
                return new CaseE<TCondition, TResult>(this, @case, then);
            }

            protected IEnumerable<CaseE<TCondition, TResult>> AllCases()
            {
                for (var @case = this; @case.PreviousCase != null; @case = @case.PreviousCase)
                {
                    yield return @case;
                }
            }

            public TResult End(TResult func, Type sourceType, Type resultType)
            {
                var cases = AllCases().Reverse();

                var typedMatch = MatchMethodInfo.MakeGenericMethod(sourceType, resultType);

                var p = Expression.Parameter(sourceType, "x");
                LambdaExpression start =
                    Expression.Lambda(Expression.Call(typedMatch, p, Expression.Constant(resultType.DefaultValue(), resultType)), p);

                var caseCaluseType = typeof(CaseClause<,>).MakeGenericType(sourceType, resultType);
                var fromSourceToResultFuncType = typeof(Func<,>).MakeGenericType(sourceType, resultType);
                var caseMethod = caseCaluseType.GetMethod("Case", new[]
                {
                    typeof(Func<,>).MakeGenericType(sourceType, typeof(bool)),
                    fromSourceToResultFuncType,
                });

                var forCaseExpression = cases.Aggregate(start.Body,
                    (current, @case) => Expression.Call(current, caseMethod, @case.Condition, @case.Result));

                var endMethodInfo = caseCaluseType.GetMethod("End", new[] { fromSourceToResultFuncType });

                Expression e = Expression.Call(forCaseExpression, endMethodInfo, func);
                var result = Expression.Lambda(e, start.Parameters);
                return (TResult) result;
            }
        }

        public class TypedCaseE<T, TResult> : CaseE<Expression<Func<T, bool>>, Expression<Func<T, TResult>>>
        {
            internal TypedCaseE(TypedCaseE<T, TResult> previousCase, Expression<Func<T, bool>> condition,
                Expression<Func<T, TResult>> result)
                : base(previousCase, condition, result)
            {
            }

            internal TypedCaseE()
            {
            }

            public new TypedCaseE<T, TResult> Case(Expression<Func<T, bool>> @case, Expression<Func<T, TResult>> then)
            {
                return new TypedCaseE<T, TResult>(this, @case, then);
            }

            public TypedCaseE<T, TResult> Case<T1>(Expression<Func<T1, TResult>> func1)
            {
                Expression<Func<T, TResult>> expression = x => func1.Apply((T1) (object) x);
                return new TypedCaseE<T, TResult>(this, x => x is T1, expression.InlineApply());
            }

            public TypedCaseE<T, TResult> Case(Type t1, LambdaExpression expression)
            {
                ParameterExpression p = Expression.Parameter(typeof(T));
                Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(Expression.TypeIs(p, t1), p);


                ParameterExpression p1 = Expression.Parameter(typeof(T));
                var selector = expression.Body.Replace(x => x == expression.Parameters[0], Expression.Convert(p1, t1));
                var selectorLambda = Expression.Lambda(Expression.Convert(selector, typeof(TResult)), p1);

                return new TypedCaseE<T, TResult>(this, lambda, (Expression<Func<T, TResult>>) selectorLambda);
            }

            public Expression<Func<T, TResult>> End(Expression<Func<T, TResult>> end)
            {
                return base.End(end, typeof(T), typeof(TResult));
            }
        }

        public static LambdaExpression GenerateMatchCaseExpression(
            Type resultType, IEnumerable<LambdaExpression> filterExpressions,
            IEnumerable<LambdaExpression> valueExpressions, ParameterExpression cellsParameter)
        {
            var cases = filterExpressions.Zip(valueExpressions, (@case, then) => (Case: @case, Then: then));
            return GenerateMatchCaseExpression(resultType, cellsParameter, cases);
        }

        public static LambdaExpression GenerateMatchCaseExpression(
            Type resultType, ParameterExpression parameter,
            IEnumerable<(LambdaExpression Case, LambdaExpression Then)> cases)
        {
            var seed = MatchE();
            var aggrergated = cases
                .Aggregate(seed, (@case, caseModel) => @case.Case(caseModel.Case, caseModel.Then));
            var endExpression = Expression.Lambda(Expression.Constant(resultType.DefaultValue(), resultType), parameter);

            return aggrergated.End(endExpression, parameter.Type, resultType);
        }
    }
}

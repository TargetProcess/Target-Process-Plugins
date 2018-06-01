using System.Collections.Generic;
using Tp.Core;
using Tp.Core.Annotations;
using Tp.Core.Expressions;
using Tp.Core.Expressions.Visitors;

// ReSharper disable once CheckNamespace

namespace System.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        public static Maybe<T> TryEval<T>(this Expression expression)
        {
            expression = expression.PartialEval();
            var constantExpression = expression as ConstantExpression;
            return constantExpression?.Value is T ? Maybe.Return((T) constantExpression.Value) : Maybe.Nothing;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<TArg0, TResult>> Expr<TArg0, TResult>([NotNull] this Expression<Func<TArg0, TResult>> expr) =>
            expr;

        [Pure]
        [NotNull]
        public static Expression<Func<TArg0, TArg1, TResult>> Expr<TArg0, TArg1, TResult>(
                [NotNull] this Expression<Func<TArg0, TArg1, TResult>> expr) =>
            expr;

        [Pure]
        public static bool IsConstantNull([NotNull] this Expression e) =>
            IsConstant(e, null);

        [Pure]
        public static bool IsConstant([NotNull] this Expression e, [CanBeNull] object constant) =>
            e.NodeType == ExpressionType.Constant && Equals(((ConstantExpression) e).Value, constant);

        /// <summary>
        /// Convert a Lambda expression to typed one, adding Convert calls if necessary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        [Pure]
        [NotNull]
        public static Expression<Func<T, TResult>> Convert<T, TResult>([NotNull] this LambdaExpression e) =>
            TryConvert<T, TResult>(e).Value;

        [Pure]
        public static Maybe<Expression<Func<T, TResult>>> TryConvert<T, TResult>([NotNull] this LambdaExpression e)
        {
            var maybeLambdaWithParameterConverted = TryConvertLambdaParameter<T>(e);
            return maybeLambdaWithParameterConverted.Bind<LambdaExpression, Expression<Func<T, TResult>>>(lambdaWithParameterConverted =>
            {
                var result = lambdaWithParameterConverted;

                var needResultCast = e.Body.Type != typeof(TResult);
                if (needResultCast && !(typeof(TResult).IsAssignableFrom(e.Body.Type) || e.Body.Type.IsAssignableFrom(typeof(TResult))))
                {
                    return Maybe.Nothing;
                }

                if (needResultCast)
                {
                    var newBody = Expression.Convert(result.Body, typeof(TResult));
                    result = Expression.Lambda(newBody, result.Parameters);
                }

                // e.Body.Type == TResult but e.ReturnType is some supertype of TResult
                if (result.ReturnType != typeof(TResult))
                {
                    return Expression.Lambda<Func<T, TResult>>(result.Body, result.Parameters);
                }

                return (Expression<Func<T, TResult>>) result;
            });
        }

        [Pure]
        public static Maybe<LambdaExpression> TryConvertLambdaParameter<T>([NotNull] this LambdaExpression e)
        {
            return TryConvertLambdaParameter(e, typeof(T));
        }

        public static Maybe<LambdaExpression> TryConvertLambdaParameter(this LambdaExpression e, Type type)
        {
            var oldParameter = e.Parameters[0];

            var needParameterCast = oldParameter.Type != type;
            if (needParameterCast && !(type.IsAssignableFrom(oldParameter.Type) || oldParameter.Type.IsAssignableFrom(type)))
            {
                return Maybe.Nothing;
            }

            if (needParameterCast)
            {
                var newParameter = Expression.Parameter(type, oldParameter.Name);
                var newBody = Replace(e.Body, x => x == oldParameter, Expression.Convert(newParameter, oldParameter.Type));
                return Expression.Lambda(newBody, newParameter);
            }

            return e;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<TNewParam, TResult>> ReplaceLambdaParameter<TNewParam, TResult>(
            [NotNull] this LambdaExpression e,
            [NotNull] LambdaExpression replacement)
        {
            var oldParameter = e.Parameters[0];
            var newParameter = replacement.Parameters[0];

            var newBody = e.Body.Replace(x => x == oldParameter, replacement.Body);
            var result = Expression.Lambda(newBody, newParameter);

            return (Expression<Func<TNewParam, TResult>>) result;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T, TResult>> MakeSingleParameter<T, TResult>(this Expression<Func<T, T, TResult>> selector)
        {
            var what = selector.Parameters[1];
            var with = selector.Parameters[0];
            var newBody = selector.Body.Replace(x => x == what, with);
            return Expression.Lambda<Func<T, TResult>>(newBody, with);
        }

        [Pure]
        [NotNull]
        public static T Replace<T>(this T @in, Func<Expression, bool> predicate, Expression with) where T : Expression
        {
            return Replace(@in, e => predicate(e) ? Maybe.Just(with) : Maybe.Nothing);
        }

        [Pure]
        [NotNull]
        public static T Replace<T>([NotNull] this T @in, Func<Expression, Maybe<Expression>> replacement) where T : Expression
        {
            return (T) new Replacer(replacement).Visit(@in);
        }

        [Pure]
        [NotNull]
        public static Expression ApplyLambdaParameterExpressions(this LambdaExpression lambda, params Expression[] arguments)
        {
            return lambda.Apply(arguments.AsEnumerable());
        }

        [Pure]
        [NotNull]
        internal static Expression Apply(this LambdaExpression lambda, IEnumerable<Expression> arguments)
        {
            arguments = arguments.ToList();

            if (lambda.Parameters.Count != arguments.Count())
            {
                throw new ArgumentException($"Expected {lambda.Parameters.Count} parameters, given {arguments.Count()} arguments");
            }

            return lambda.Parameters.Zip(arguments, (parameter, argument) => new { parameter, argument })
                .Aggregate(lambda.Body, (body, pair) => body.Replace(x => x == pair.parameter, pair.argument));
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expression, T1 t1, T2 t2,
            T3 t3,
            T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16)
        {
            return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression, T1 t1, T2 t2, T3 t3,
            T4 t4,
            T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15)
        {
            return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression, T1 t1, T2 t2, T3 t3,
            T4 t4, T5 t5,
            T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4,
            T5 t5,
            T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5,
            T6 t6,
            T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5,
            T6 t6, T7 t7,
            T8 t8, T9 t9, T10 t10, T11 t11)
        {
            return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6,
            T7 t7,
            T8 t8, T9 t9, T10 t10)
        {
            return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7,
            T8 t8,
            T9 t9)
        {
            return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7,
            T8 t8)
        {
            return expression.Compile()(t1, t2, t3, t4, t5, t6, t7, t8);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, T6, T7, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression,
            T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            return expression.Compile()(t1, t2, t3, t4, t5, t6, t7);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, T6, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression,
            T1 t1,
            T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            return expression.Compile()(t1, t2, t3, t4, t5, t6);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, T5, TResult>(this Expression<Func<T1, T2, T3, T4, T5, TResult>> expression, T1 t1, T2 t2,
            T3 t3, T4 t4, T5 t5)
        {
            return expression.Compile()(t1, t2, t3, t4, t5);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, T4, TResult>(this Expression<Func<T1, T2, T3, T4, TResult>> expression, T1 t1, T2 t2, T3 t3,
            T4 t4)
        {
            return expression.Compile()(t1, t2, t3, t4);
        }

        [Pure]
        public static TResult Apply<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> expression, T1 t1, T2 t2, T3 t3)
        {
            return expression.Compile()(t1, t2, t3);
        }

        [Pure]
        public static TResult Apply<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, T1 t1, T2 t2)
        {
            return expression.Compile()(t1, t2);
        }

        [Pure]
        public static TResult Apply<T1, TResult>(this Expression<Func<T1, TResult>> expression, T1 t1)
        {
            return expression.Compile()(t1);
        }

        [Pure]
        public static TResult Apply<TResult>(this Expression<Func<TResult>> expression)
        {
            return expression.Compile()();
        }

        [Pure]
        [NotNull]
        public static Expression<Func<TResult>> PartialApply<T1, TResult>(this Expression<Func<T1, TResult>> expression, T1 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, TResult>> PartialApply<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression,
            T2 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, TResult>> PartialApply<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> expression,
            T3 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, TResult>> PartialApply<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> expression, T4 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, T4, TResult>> PartialApply<T1, T2, T3, T4, T5, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, TResult>> expression, T5 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, T4, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, T4, T5, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression, T6 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, T4, T5, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression, T7 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, T4, T5, T6, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, T8 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> PartialApply<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, T9 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> PartialApply
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
            (this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, T10 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> PartialApply
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
                this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression, T11 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> PartialApply
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
                this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression, T12 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> PartialApply
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
                this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression, T13 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> PartialApply
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
                this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression, T14 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> PartialApply
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
                this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression, T15 paramValue)
        {
            var lambda = PartialApplyLambda(expression, paramValue);
            return (Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>) lambda;
        }

        [Pure]
        [NotNull]
        private static LambdaExpression PartialApplyLambda<TParam2>([NotNull] LambdaExpression expression, TParam2 paramValue)
        {
            var lastParameter = expression.Parameters.Last();
            var constant = Expression.Constant(paramValue, lastParameter.Type);
            var result = Replace(expression.Body, x => x == lastParameter as Expression, constant);
            var lambda = Expression.Lambda(result, expression.Parameters.Take(expression.Parameters.Count - 1));
            return lambda;
        }

        [Pure]
        [NotNull]
        public static T InlineApply<T>([NotNull] this T expression) where T : Expression
        {
            var invoker = new InlineApplyVisitor();
            Expression visit = invoker.Visit(expression);
            return (T) visit;
        }

        [Pure]
        [NotNull]
        public static T Inline<T>([NotNull] this T expression, params object[] inlineEnvironment) where T : Expression
        {
            if (inlineEnvironment.Any(x => x == null))
            {
                throw new ArgumentException("Inline environment element can't be null");
            }

            return (T) new InlineVisitor(inlineEnvironment).Visit(expression);
        }

        [Pure]
        [NotNull]
        public static Expression EvalBooleanConstants([NotNull] this Expression expression)
        {
            return new BooleanEvaluator().Visit(expression);
        }

        [Pure]
        [NotNull]
        public static T ProtectFromNullReference<T>([NotNull] this T expression, ISet<Expression> notNullExpressions = null) where T : Expression
        {
            return (T) (new ProtectFromNullReferenceVisitor(notNullExpressions).Visit(expression));
        }

        [Pure]
        [NotNull]
        public static T ProtectFromDivideByZero<T>([NotNull] this T expression) where T : Expression
        {
            return (T) (new ProtectFromDivideByZeroVisitor().Visit(expression));
        }

        [Pure]
        [NotNull]
        public static T FixFloatEqualityComparison<T>([NotNull] this T expression, float delta) where T : Expression
        {
            return (T) (new FixFloatComparisonVisitor(delta).Visit(expression));
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T, bool>> Negate<T>([NotNull] this Expression<Func<T, bool>> expr)
        {
            if (expr.Body.NodeType == ExpressionType.Not)
            {
                return Expression.Lambda<Func<T, bool>>(((UnaryExpression) expr.Body).Operand, expr.Parameters);
            }
            return Expression.Lambda<Func<T, bool>>(Expression.Not(expr.Body), expr.Parameters);
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T, bool>> CombineAnd<T>(
                [NotNull] this Expression<Func<T, bool>> head,
                [NotNull] [ItemNotNull] params Expression<Func<T, bool>>[] tail) =>
            Combine(head, tail, Expression.AndAlso);

        [Pure]
        [NotNull]
        public static Expression<Func<T, bool>> CombineAnd<T>(
                [NotNull] this Expression<Func<T, bool>> head,
                [NotNull] [ItemNotNull] IReadOnlyCollection<Expression<Func<T, bool>>> tail) =>
            Combine(head, tail, Expression.AndAlso);

        [Pure]
        [NotNull]
        public static Expression<Func<T, bool>> CombineOr<T>(
                [NotNull] this Expression<Func<T, bool>> head,
                [NotNull] [ItemNotNull] params Expression<Func<T, bool>>[] tail) =>
            Combine(head, tail, Expression.OrElse);

        [Pure]
        [NotNull]
        public static Expression<Func<T, bool>> CombineOr<T>(
            [NotNull] this Expression<Func<T, bool>> head,
            [NotNull] [ItemNotNull] IReadOnlyCollection<Expression<Func<T, bool>>> tail) =>
            Combine(head, tail, Expression.OrElse);

        [Pure]
        [NotNull]
        public static Expression<Func<T, bool>> CombineAnd<T>([NotNull] [ItemNotNull] this IEnumerable<Expression<Func<T, bool>>> expressions)
        {
            var exprs = expressions.ToReadOnlyCollection();
            return CombineAnd(exprs.First(), exprs.Skip(1).ToArray());
        }

        [Pure]
        [NotNull]
        public static Expression<Func<T, bool>> CombineOr<T>([NotNull] [ItemNotNull] this IEnumerable<Expression<Func<T, bool>>> expressions)
        {
            var exprs = expressions.ToReadOnlyCollection();
            return CombineOr(exprs.First(), exprs.Skip(1).ToArray());
        }

        [Pure]
        [NotNull]
        public static Expression CombineOr([NotNull] [ItemNotNull] this IEnumerable<Expression> expressions) =>
            expressions.Aggregate(Expression.OrElse);

        [Pure]
        [NotNull]
        public static Expression CombineAnd([NotNull] [ItemNotNull] this IEnumerable<Expression> expressions) =>
            expressions.Aggregate(Expression.AndAlso);

        [Pure]
        [NotNull]
        private static Expression<Func<T, bool>> Combine<T>(
            [NotNull] Expression<Func<T, bool>> head,
            [NotNull] [ItemNotNull] IReadOnlyCollection<Expression<Func<T, bool>>> tail,
            [NotNull] [InstantHandle] Func<Expression, Expression, BinaryExpression> combiner)
        {
            Expression<Func<T, bool>> tree = BuildBalancedExpressionTree(head, tail, combiner);
            return (Expression<Func<T, bool>>)Expression.Lambda(combiner(head.Body, tree.Body), head.Parameters);
        }

        [Pure]
        [NotNull]
        private static Expression<Func<T, bool>> BuildBalancedExpressionTree<T>(
            [NotNull] Expression<Func<T, bool>> head,
            [NotNull] [ItemNotNull] IReadOnlyCollection<Expression<Func<T, bool>>> nodes,
            [NotNull] [InstantHandle] Func<Expression, Expression, BinaryExpression> combiner)
        {
            if (nodes.IsNullOrEmpty())
            {
                return head;
            }
            else if (nodes.Count == 1)
            {
                var lambda = LambdaSubstituter.ReplaceParameters(nodes.Single(), head.Parameters);
                return Expression.Lambda<Func<T, bool>>(lambda, head.Parameters);
            }

            int halfTreeSize = nodes.Count / 2;
            var leftTree = BuildBalancedExpressionTree(head, nodes.Take(halfTreeSize).ToArray(), combiner);
            var rightTree = BuildBalancedExpressionTree(head, nodes.Skip(halfTreeSize).ToArray(), combiner);
            var tree = Expression.Lambda<Func<T, bool>>(combiner(leftTree.Body, rightTree.Body), head.Parameters);
            return tree;
        }


        [Pure]
        [NotNull]
        public static Expression ChangeCtorType<TBase, TDerived>([NotNull] this Expression lambda)
        {
            return new CtorTypeChanger<TBase, TDerived>().Visit(lambda);
        }

        [Pure]
        [NotNull]
        public static Expression ChangeCtorType([NotNull] this Expression lambda,
            [NotNull] Type baseType, [NotNull] Type derivedType)
        {
            return new CtorTypeChanger(baseType, derivedType).Visit(lambda);
        }

        [Pure]
        [NotNull]
        public static IEnumerable<Expression> TraversePreOrder([NotNull] this Expression expression)
        {
            var visitor = new PreOrderTraverseVisitor();
            return visitor.Traverse(expression);
        }

        [Pure]
        [NotNull]
        public static Expression Transform(this Expression expression, params Func<Expression, Expression>[] transformations)
        {
            return transformations.Aggregate(expression, (current, transformation) => transformation(current));
        }
    }
}

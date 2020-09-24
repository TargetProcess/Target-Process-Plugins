using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using StructureMap;
using Tp.Core.Annotations;
using Tp.Core.Features;
using IContainer = StructureMap.IContainer;
using Res = System.Linq.Dynamic.Res;

namespace Tp.Core.Linq
{
    public static class SharedParserUtils
    {
        // Performance tip: try using nullable returns instead of Maybe because this parsing code is called on hot path

        public static long EntityCustomValuePromoteToIntCounter;

        public static readonly IReadOnlyList<MethodInfo> EnumerableMethods =
            typeof(Enumerable)
                .GetMethods()
                .ToReadOnlyCollection();

        private static readonly MethodInfo ContainsMethod =
            Reflect<int[]>.GetMethod(x => x.Contains(default(int))).GetGenericMethodDefinition();

        internal static readonly IReadOnlyDictionary<string, Type> PredefinedTypesByName = new[]
        {
            typeof(Object),
            typeof(Boolean),
            typeof(Char),
            typeof(String),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(Decimal),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Math),
            typeof(Convert),
            typeof(DateTime?),
            typeof(Enum),
            typeof(SafeConvert)
        }.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

        public static readonly Expression TrueLiteral = Expression.Constant(true);
        public static readonly Expression FalseLiteral = Expression.Constant(false);
        public static readonly Expression NullLiteral = Expression.Constant(null);

        [Pure]
        public static string GetTypeName([NotNull] Type type)
        {
            var baseType = GetNonNullableType(type);

            var displayName = baseType.GetCustomAttribute<DisplayNameAttribute>();
            var result = displayName.HasValue && !displayName.Value.DisplayName.IsNullOrEmpty()
                ? displayName.Value.DisplayName
                : baseType.Name;

            if (type != baseType)
            {
                result += '?';
            }

            return result;
        }

        [Pure]
        [CanBeNull]
        internal static Type FindGenericType([NotNull] Type generic, [NotNull] Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == generic) return type;
                if (generic.IsInterface)
                {
                    foreach (var interfaceType in type.GetInterfaces())
                    {
                        var found = FindGenericType(generic, interfaceType);
                        if (found != null) return found;
                    }
                }

                type = type.BaseType;
            }

            return null;
        }

        public static void CheckAndPromoteOperands(
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
            [NotNull] IReadOnlyDictionary<Expression, string> literals,
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
            [NotNull] Type signatures,
            [NotNull] string opName,
            [NotNull] ref Expression left, [NotNull] ref Expression right,
            int errorPos)
        {
            var args = new[] { left, right };
            if (!GetAppropriateMethod(literals, signatures, "F", false, args).IsSingle)
            {
                throw IncompatibleOperandsError(opName, left, right, errorPos);
            }

            left = args[0];
            right = args[1];
        }

        [Pure]
        [NotNull]
        internal static Exception IncompatibleOperandsError(
            [NotNull] string opName, [NotNull] Expression left, [NotNull] Expression right, int pos)
        {
            return new ParseException(Res.IncompatibleOperands(opName, GetTypeName(left.Type), GetTypeName(right.Type)), pos);
        }

        [NotNull]
        [Pure]
        internal static Expression ConvertNullableBoolToBoolExpression([NotNull] Expression expr)
        {
            return expr.Type == typeof(bool?)
                ? Expression.Equal(expr, Expression.Constant(true, typeof(bool?)))
                : expr;
        }

        internal static Expression EqualizeTypesAndCombine(
            IReadOnlyDictionary<Expression, string> literals, Expression expr1, Expression expr2, int errorPos,
            Func<Expression, Expression, Expression> combineResults)
        {
            if (expr1.Type != expr2.Type)
            {
                var expr1As2 = expr2 != NullLiteral ? PromoteExpression(literals, expr1, expr2.Type, true) : null;
                var expr2As1 = expr1 != NullLiteral ? PromoteExpression(literals, expr2, expr1.Type, true) : null;
                if (expr1As2 != null && expr2As1 == null)
                {
                    expr1 = expr1As2;
                }
                else if (expr2As1 != null && expr1As2 == null)
                {
                    expr2 = expr2As1;
                }
                else
                {
                    var type1 = expr1 != NullLiteral ? expr1.Type.Name : "null";
                    var type2 = expr2 != NullLiteral ? expr2.Type.Name : "null";
                    if (expr1As2 != null)
                    {
                        throw new ParseException(Res.BothTypesConvertToOther(type1, type2), errorPos);
                    }

                    throw new ParseException(Res.NeitherTypeConvertsToOther(type1, type2), errorPos);
                }
            }

            return combineResults(expr1, expr2);
        }

        [CanBeNull]
        public static Expression PromoteExpression(
            [NotNull] IReadOnlyDictionary<Expression, string> literals,
            [NotNull] Expression expr,
            [NotNull] Type type,
            bool exact)
        {
            if (expr.Type == type) return expr;

            if (expr is ConstantExpression ce)
            {
                if (ce == NullLiteral)
                {
                    if (!type.IsValueType || IsNullableType(type))
                        return Expression.Constant(null, type);
                }
                else
                {
                    if (literals.TryGetValue(ce, out var text))
                    {
                        var target = GetNonNullableType(type);
                        object value = null;
                        switch (Type.GetTypeCode(ce.Type))
                        {
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                value = ParseNumber(text, target);
                                break;
                            case TypeCode.Double:
                                if (target == typeof(decimal)) value = ParseNumber(text, target);
                                break;
                            case TypeCode.String:
                                value = ParseEnum(text, target);
                                break;
                        }

                        if (value != null)
                            return Expression.Constant(value, type);
                    }
                }
            }

            if (expr is UnaryExpression unary
                && expr.NodeType == ExpressionType.Convert
                && IsEntityCustomValue(expr.Type)
                && type == typeof(int)
                && TpFeature.AllowToFilterByEntityCustomFieldDirectly.IsEnabled())
            {
                // Backwards compatibility for rarely used cases like `?where=SomeEntityCF != 40`.
                // See `/adr/2020-09-14 - Entity Custom Values in API v2.md` for details.
                Interlocked.Increment(ref EntityCustomValuePromoteToIntCounter);
                return Expression.Convert(unary.Operand, typeof(int));
            }

            if (IsCompatibleWith(expr.Type, type))
            {
                if (type.IsValueType || exact) return Expression.Convert(expr, type);
                return expr;
            }

            if (expr.Type == typeof(char) && type == typeof(string))
            {
                return Expression.Call(expr, nameof(string.ToString), Type.EmptyTypes);
            }

            return null;
        }

        // Type name is used instead of type because parser's assembly doesn't have direct reference to an assembly containing that type.
        // Yes, it's ugly, and a proper way to do that would be to introduce an extension point
        // which allows to configure expression type conversions, but that seems to be an over-kill for this minor functionality.
        // TODO: However, may want to return to this idea if more similar extension points are needed in the future.
        internal static bool IsEntityCustomValue(Type type)
        {
            return type.Name == "EntityCustomValue";
        }

        [Pure]
        internal static SingleMember<T> FindBestMethod<T>(
            IReadOnlyDictionary<Expression, string> literals,
            [NotNull] [ItemNotNull] IEnumerable<T> candidates,
            [CanBeNull] string methodName,
            [NotNull] [ItemNotNull] Expression[] methodArgs)
            where T : MethodBase
        {
            var applicableMethods = candidates
                .Where(m => methodName is null || IsMethodSuitByName(methodName, m))
                .Select(m => new MethodData<T> { MethodBase = m, Parameters = m.GetParameters() })
                .Where(m => IsApplicable(literals, m, methodArgs))
                .ToArray();

            if (applicableMethods.Length > 1)
            {
                applicableMethods = applicableMethods
                    .Where(m => applicableMethods.All(n => m == n || IsBetterThan(methodArgs, m, n)))
                    .ToArray();
            }

            if (applicableMethods.Length == 1)
            {
                var md = applicableMethods[0];
                for (var i = 0; i < methodArgs.Length; i++)
                {
                    methodArgs[i] = md.Args[i];
                }

                return new SingleMember<T>(md.MethodBase);
            }

            if (applicableMethods.Length > 1)
            {
                return SingleMember<T>.Several;
            }

            return SingleMember<T>.None;
        }

        private static bool IsMethodSuitByName(
            [NotNull] string methodName, [NotNull] MemberInfo m)
        {
            // TODO: should be exposed as Type System meta API
            var aliases = new[] { m.Name }.Concat(m.GetCustomAttributes<DynamicExpressionAliasAttribute>().Select(x => x.Name));
            return aliases.Contains(methodName, StringComparer.InvariantCultureIgnoreCase);
        }

        private static bool IsApplicable<T>(
            IReadOnlyDictionary<Expression, string> literals,
            [NotNull] MethodData<T> method, [NotNull] [ItemNotNull] Expression[] args)
            where T : MethodBase
        {
            if (method.Parameters.Length != args.Length) return false;
            var promotedArgs = new Expression[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var pi = method.Parameters[i];
                if (pi.IsOut) return false;
                var promoted = PromoteExpression(literals, args[i], pi.ParameterType, false);
                if (promoted == null) return false;
                promotedArgs[i] = promoted;
            }

            method.Args = promotedArgs;
            return true;
        }

        [Pure]
        internal static bool IsNumericType([NotNull] Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        [Pure]
        internal static bool IsEnumType([NotNull] Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }

        [Pure]
        public static bool IsNullableType([NotNull] Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        [Pure]
        [NotNull]
        private static Type GetNonNullableType(
            [NotNull] Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        [Pure]
        [NotNull]
        public static Expression GenerateIn(
            [NotNull] Expression operand,
            [NotNull] IEnumerable<Expression> args)
        {
            return Expression.Call(
                ContainsMethod.MakeGenericMethod(operand.Type),
                Expression.NewArrayInit(
                    operand.Type,
                    args.Select(x => x.Type == operand.Type ? x : Expression.Convert(x, operand.Type))),
                operand);
        }

        [Pure]
        [NotNull]
        public static Expression GenerateConditional(
            IReadOnlyDictionary<Expression, string> literals,
            Expression test,
            Expression expr1, Expression expr2, int errorPos)
        {
            test = ConvertNullableBoolToBoolExpression(test);
            if (test.Type != typeof(bool))
            {
                throw new ParseException(Res.FirstExprMustBeBool, errorPos);
            }

            return EqualizeTypesAndCombine(literals, expr1, expr2, errorPos, (e1, e2) => Expression.Condition(test, e1, e2));
        }

        [Pure]
        [NotNull]
        public static Expression GenerateStringConcat(
            [NotNull] Expression left, [NotNull] Expression right)
        {
            return Expression.Call(
                null,
                // ReSharper disable once AssignNullToNotNullAttribute
                typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(object), typeof(object) }),
                new[] { left, right });
        }

        [NotNull]
        public static Expression GenerateStaticMethodCall(
            int errorPosition,
            [NotNull] string methodName,
            [NotNull] Expression left, [NotNull] Expression right)
        {
            var method = GetStaticMethod(methodName, left, right);
            if (method == null)
            {
                throw new ParseException(Res.NoApplicableMethod(methodName, left.Type.Name), errorPosition);
            }

            return Expression.Call(null, method, new[] { left, right });
        }

        [CanBeNull]
        private static MethodInfo GetStaticMethod(
            [NotNull] string methodName,
            [NotNull] Expression left, [NotNull] Expression right)
        {
            return left.Type.GetMethod(methodName, new[] { left.Type, right.Type });
        }

        [CanBeNull]
        private static object ParseNumber([NotNull] string text, [NotNull] Type type)
        {
            // For enums Type.GetTypeCode gets numeric underlying type code, but we expect enum constants as strings only.
            // See ParseEnum for details.
            if (type.IsEnum) return null;

            switch (Type.GetTypeCode(GetNonNullableType(type)))
            {
                case TypeCode.SByte:
                    if (SByte.TryParse(text, out var sb) || SByte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out sb))
                        return sb;
                    break;
                case TypeCode.Byte:
                    if (Byte.TryParse(text, out var b) || Byte.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out b)) return b;
                    break;
                case TypeCode.Int16:
                    if (Int16.TryParse(text, out var s) || Int16.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out s))
                        return s;
                    break;
                case TypeCode.UInt16:
                    if (UInt16.TryParse(text, out var us) || UInt16.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out us))
                        return us;
                    break;
                case TypeCode.Int32:
                    if (Int32.TryParse(text, out var i) || Int32.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out i)) return i;
                    break;
                case TypeCode.UInt32:
                    if (UInt32.TryParse(text, out var ui) || UInt32.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out ui))
                        return ui;
                    break;
                case TypeCode.Int64:
                    if (Int64.TryParse(text, out var l) || Int64.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out l)) return l;
                    break;
                case TypeCode.UInt64:
                    if (UInt64.TryParse(text, out var ul) || UInt64.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out ul))
                        return ul;
                    break;
                case TypeCode.Single:
                    if (Single.TryParse(text, out var f) || Single.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out f))
                        return f;
                    break;
                case TypeCode.Double:
                    if (Double.TryParse(text, out var d) || Double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                        return d;
                    break;
                case TypeCode.Decimal:
                    if (Decimal.TryParse(text, out var e) || Decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out e))
                        return e;
                    break;
            }

            return null;
        }

        [CanBeNull]
        private static object ParseEnum([NotNull] string name, [NotNull] Type type)
        {
            if (type.IsEnum)
            {
                var memberInfos = type.FindMembers(MemberTypes.Field,
                    BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static,
                    Type.FilterNameIgnoreCase, name);
                if (memberInfos.Length != 0) return ((FieldInfo) memberInfos[0]).GetValue(null);
            }

            return null;
        }

        private static bool IsBetterThan<T>(
            [NotNull] [ItemNotNull] Expression[] args,
            [NotNull] MethodData<T> m1, [NotNull] MethodData<T> m2)
            where T : MethodBase
        {
            var better = false;
            for (var i = 0; i < args.Length; i++)
            {
                var c = CompareConversions(args[i].Type,
                    m1.Parameters[i].ParameterType,
                    m2.Parameters[i].ParameterType);
                if (c < 0) return false;
                if (c > 0) better = true;
            }

            return better;
        }

        // Return 1 if s -> t1 is a better conversion than s -> t2
        // Return -1 if s -> t2 is a better conversion than s -> t1
        // Return 0 if neither conversion is better
        private static int CompareConversions(
            [NotNull] Type s, [NotNull] Type t1, [NotNull] Type t2)
        {
            if (t1 == t2) return 0;
            if (s == t1) return 1;
            if (s == t2) return -1;
            var t1T2 = IsCompatibleWith(t1, t2);
            var t2T1 = IsCompatibleWith(t2, t1);
            if (t1T2 && !t2T1) return 1;
            if (t2T1 && !t1T2) return -1;
            if (IsSignedIntegralType(t1) && IsUnsignedIntegralType(t2)) return 1;
            if (IsSignedIntegralType(t2) && IsUnsignedIntegralType(t1)) return -1;
            return 0;
        }

        [Pure]
        private static bool IsSignedIntegralType([NotNull] Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        [Pure]
        private static bool IsUnsignedIntegralType([NotNull] Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        [Pure]
        private static int GetNumericTypeKind([NotNull] Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum) return 0;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }

        [Pure]
        internal static SingleMember<MethodBase> GetAppropriateMethod(
            IReadOnlyDictionary<Expression, string> literals,
            [NotNull] Type type,
            [NotNull] string methodName, bool staticAccess,
            [NotNull] [ItemNotNull] Expression[] args)
        {
            var flags = BindingFlags.Public | BindingFlags.DeclaredOnly | (staticAccess ? BindingFlags.Static : BindingFlags.Instance)
                | BindingFlags.IgnoreCase;

            foreach (MemberInfo[] members in SelfAndBaseTypesMemo.Apply(type).Select(t => t.GetMethods(flags)))
            {
                var bestMethod = FindBestMethod(literals, members.Cast<MethodBase>(), methodName, args);
                if (bestMethod.HasAny)
                {
                    return bestMethod;
                }
            }

            return SingleMember<MethodBase>.None;
        }

        [Pure]
        internal static SingleMember<MethodBase> FindIndexer(
            IReadOnlyDictionary<Expression, string> literals,
            [NotNull] Type type,
            [NotNull] [ItemNotNull] Expression[] args)
        {
            foreach (var t in SelfAndBaseTypesMemo.Apply(type))
            {
                var members = t.GetDefaultMembers();
                if (members.Length != 0)
                {
                    var methods = members
                        .OfType<PropertyInfo>()
                        .Select(p => (MethodBase) p.GetGetMethod())
                        .WhereNotNull();

                    var bestMethod = FindBestMethod(literals, methods, null, args);
                    if (bestMethod.HasAny)
                    {
                        return bestMethod;
                    }
                }
            }

            return SingleMember<MethodBase>.None;
        }

        [Pure]
        internal static MemberInfo FindPropertyOrField(
            [NotNull] this Type type, [NotNull] string memberName, bool staticAccess)
        {
            var flags = BindingFlags.Public | BindingFlags.DeclaredOnly |
                (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
            return (from t in SelfAndBaseTypesMemo.Apply(type)
                select t.FindMembers(MemberTypes.Property | MemberTypes.Field, flags, Type.FilterNameIgnoreCase, memberName)
                into members
                where members.Length != 0
                select members[0]).FirstOrDefault();
        }

        public static readonly Memoizator<Type, IReadOnlyList<Type>> SelfAndBaseTypesMemo =
            new Memoizator<Type, IReadOnlyList<Type>>(
                type =>
                {
                    if (type.IsInterface)
                    {
                        var types = new HashSet<Type>();
                        AddInterface(types, type);
                        return types.ToReadOnlyCollection();
                    }

                    return SelfAndBaseClasses(type).ToReadOnlyCollection();

                    IEnumerable<Type> SelfAndBaseClasses(Type t)
                    {
                        while (t != null)
                        {
                            yield return t;
                            t = t.BaseType;
                        }
                    }

                    void AddInterface(HashSet<Type> types, Type t)
                    {
                        if (types.Contains(t))
                        {
                            return;
                        }

                        types.Add(t);

                        foreach (var ti in t.GetInterfaces())
                        {
                            AddInterface(types, ti);
                        }
                    }
                });

        [CanBeNull]
        // special case for protected nullable properties
        // for x==null?null:(Nullable<T>)Expr(x) returns name of Expr(x)
        private static string GetConditionName(Expression expr, int exprPos)
        {
            if (expr is ConditionalExpression conditional)
            {
                var value = ExtensionsProvider.GetValue<Expression>(conditional);
                if (value.HasValue)
                {
                    return GetPropertyName(value.Value, exprPos);
                }
            }

            return null;
        }

        [CanBeNull]
        internal static string GetPropertyName(Expression expr, int exprPos)
        {
            if (expr is MemberExpression m)
            {
                return m.Member.Name;
            }

            if (DynamicDictionary.TryGetAlias(expr, out var dynamicName))
            {
                return dynamicName;
            }

            return GetConditionName(expr, exprPos)
                ?? GetEnumerableRootName(expr, exprPos)
                ?? GetMethodName(expr);
        }

        [CanBeNull]
        // find the root of Enumerable chains and get it name
        // userstories.Where(x).select(y)=>'userstories'
        private static string GetEnumerableRootName(Expression expr, int exprPos)
        {
            if (expr is MethodCallExpression x)
            {
                if (x.Method.DeclaringType == typeof(Enumerable) && !AggregationMethodNames.Contains(x.Method.Name))
                {
                    var arg = x.Arguments.First();
                    return GetPropertyName(arg, exprPos);
                }
            }

            return null;
        }

        // Please, order by usage frequency to microoptimize Enumerable.Contains().
        private static readonly string[] AggregationMethodNames = new[]
        {
            nameof(Enumerable.Count), nameof(Enumerable.Min), nameof(Enumerable.Max),
            nameof(Enumerable.Sum), nameof(Enumerable.Average)
        }.Concat(nameof(Enumerable.Aggregate)).ToArray();

        [CanBeNull]
        private static string GetMethodName(Expression expr)
        {
            if (expr is MethodCallExpression call)
            {
                if (call.Arguments.Count == 0 || (call.Method.IsExtensionMethod() && call.Arguments.Count == 1))
                {
                    return call.Method.Name;
                }
            }

            return null;
        }

        private static bool IsCompatibleWith([NotNull] Type source, [NotNull] Type target)
        {
            if (source == target) return true;
            if (!target.IsValueType) return target.IsAssignableFrom(source);
            var st = GetNonNullableType(source);
            var tt = GetNonNullableType(target);
            if (st != source && tt == target) return false;
            var sc = st.IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            var tc = tt.IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }

                    break;
                case TypeCode.Decimal:
                    switch (tc)
                    {
                        case TypeCode.Decimal:
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }

                    break;
                default:
                    if (st == tt) return true;
                    break;
            }

            return false;
        }


        internal class SingleMember<T>
            where T : class
        {
            private readonly T _value;

            public bool HasAny { get; }
            public bool HasSeveral { get; }
            public bool IsSingle => HasAny && !HasSeveral;

            public SingleMember([NotNull] T value) : this(hasAny: true, hasSeveral: false)
            {
                _value = Argument.NotNull(nameof(value), value);
                HasAny = true;
                HasSeveral = false;
            }

            private SingleMember(bool hasAny, bool hasSeveral)
            {
                HasAny = hasAny;
                HasSeveral = hasSeveral;
            }

            [NotNull]
            public T GetSingleOrThrow()
            {
                if (HasSeveral)
                {
                    throw new InvalidOperationException($"Single value of type {typeof(T)} expected, but got several");
                }

                if (!HasAny)
                {
                    throw new InvalidOperationException($"Single value of type {typeof(T)} expected, but got none");
                }

                return _value;
            }

            public static SingleMember<T> None => new SingleMember<T>(false, false);
            public static SingleMember<T> Several => new SingleMember<T>(hasAny: true, hasSeveral: false);
        }

        private class MethodData<T>
            where T : MethodBase
        {
            public Expression[] Args;
            public T MethodBase;
            public ParameterInfo[] Parameters;
        }

        private static readonly Lazy<IReadOnlyDictionary<string, Type>> _defaultKnownTypes =
            Lazy.Create(GetKnownTypes);

        public static IReadOnlyDictionary<string, Type> DefaultKnownTypes => _defaultKnownTypes.Value;

        [Pure]
        [NotNull]
        public static IReadOnlyDictionary<string, Type> GetKnownTypes() =>
            ObjectFactory.Container
                .GetAllInstances<ITypeProvider>().SelectMany(x => x.GetKnownTypes())
                .DistinctBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);

        private static readonly Lazy<IReadOnlyCollection<MethodInfo>> _defaultExtensionMethods =
            Lazy.Create(GetExtensionMethods);

        public static IReadOnlyCollection<MethodInfo> DefaultExtensionMethods => _defaultExtensionMethods.Value;

        [Pure]
        [NotNull]
        [ItemNotNull]
        public static IReadOnlyCollection<MethodInfo> GetExtensionMethods() =>
            ObjectFactory.Container
                .GetAllInstances<IMethodProvider>()
                .SelectMany(x => x.GetExtensionMethodInfo())
                .ToList();

        private static readonly Lazy<ISurrogateGenerator> _defaultSurrogateGenerator =
            Lazy.Create(() => CreateSurrogateGenerator(ObjectFactory.Container));

        public static ISurrogateGenerator DefaultSurrogateGenerator => _defaultSurrogateGenerator.Value;

        [Pure]
        [NotNull]
        public static ISurrogateGenerator CreateSurrogateGenerator(IContainer container)
        {
            var registeredSurrogates = container
                .GetAllInstances<ISurrogateMethod>()
                .ToDictionaryOfKnownCapacity(x => x.Name, StringComparer.OrdinalIgnoreCase);

            return new LambdaSurrogateGenerator(
                () => "bi", // short for "built-in"
                (name, expression) =>
                {
                    if (registeredSurrogates.TryGetValue(name, out var method))
                    {
                        return method.GetMethodExpression(expression);
                    }

                    return Maybe<Expression>.Nothing;
                });
        }

        [NotNull]
        public static Type CreateClass(IEnumerable<DynamicProperty> properties, Type baseType = null) =>
            ClassFactory.Instance.GetDynamicClass(properties, baseType);
    }
}

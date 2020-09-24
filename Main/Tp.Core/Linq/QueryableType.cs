using System;
using Antlr4.Runtime.Sharpen;
using Tp.Core.Annotations;

namespace Tp.Core.Linq
{
    /// <summary>
    /// Represents the type information of the data analyzed by <see cref="System.Linq.Dynamic.IExpressionParser"/>
    /// when it builds expression from text.
    /// </summary>
    /// <remarks>
    /// Resource querying mechanism relies a lot on .NET expressions to represent query details, such as field access.
    /// This works well for "native", code-generated, resources like UserStoryDto.
    ///
    /// However, resources from extendable domain are not directly mapped to .NET classes, but rather represented as key-value dictionaries.
    ///
    /// Expression parser, which translates strings to .NET expressions relies on type information about resources,
    /// and sometimes we need to tell it "this resource is of type AssignableDto, but actually it's custom entity called Objective from extendable domain".
    /// </remarks>
    public class QueryableType : IEquatable<QueryableType>
    {
        private static readonly ConcurrentDictionary<(Type, string, bool), QueryableType> _extendablePool =
            new ConcurrentDictionary<(Type, string, bool), QueryableType>();

        private static readonly ConcurrentDictionary<Type, QueryableType> _nativePool = new ConcurrentDictionary<Type, QueryableType>();

        private QueryableType(
            [NotNull] Type runtimeType,
            Maybe<string> extendableDomainTypeName,
            bool isExtendableDomainCollection)
        {
            RuntimeType = Argument.NotNull(nameof(runtimeType), runtimeType);
            ExtendableDomainTypeName = extendableDomainTypeName;
            IsExtendableDomainCollection = isExtendableDomainCollection;
        }

        /// <summary>
        /// When not Nothing, specifies a string name of the resource type.
        /// </summary>
        public Maybe<string> ExtendableDomainTypeName { get; }

        public bool IsExtendableDomainCollection { get; }

        /// <summary>
        /// Underlying CLR type of resource.
        /// </summary>
        [NotNull]
        public Type RuntimeType { get; }

        [Pure]
        [NotNull]
        public static QueryableType CreateNative(Type type)
        {
            return _nativePool.GetOrAdd(type, t => new QueryableType(t, Maybe<string>.Nothing, false));
        }

        [Pure]
        [NotNull]
        public static QueryableType CreateNative<T>() => CreateNative(typeof(T));

        [Pure]
        [NotNull]
        public static QueryableType CreateExtendableDomain(Type underlyingRuntimeType, string typeName, bool isCollection = false)
        {
            return _extendablePool.GetOrAdd((underlyingRuntimeType, typeName, isCollection),
                x => new QueryableType(x.Item1, x.Item2, x.Item3));
        }

        public bool Equals(QueryableType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return RuntimeType == other.RuntimeType
                && ExtendableDomainTypeName.Equals(other.ExtendableDomainTypeName)
                && IsExtendableDomainCollection == other.IsExtendableDomainCollection;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((QueryableType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ExtendableDomainTypeName.GetHashCode();
                hashCode = (hashCode * 397) ^ IsExtendableDomainCollection.GetHashCode();
                hashCode = (hashCode * 397) ^ RuntimeType.GetHashCode();
                return hashCode;
            }
        }

        [Pure]
        public bool IsAssignableFrom([NotNull] QueryableType maybeChildType)
        {
            Argument.NotNull(nameof(maybeChildType), maybeChildType);
            if (ExtendableDomainTypeName.HasValue)
            {
                return maybeChildType.ExtendableDomainTypeName.Equals(ExtendableDomainTypeName);
            }

            return RuntimeType.IsAssignableFrom(maybeChildType.RuntimeType);
        }

        public override string ToString()
        {
            if (ExtendableDomainTypeName.HasValue)
            {
                return ExtendableDomainTypeName.Value;
            }

            return RuntimeType.Name;
        }
    }
}

using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Tp.Core;

namespace System
{
	public static class TypeHelper
	{
		public static bool IsNumeric(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}
			return IsBetween(Type.GetTypeCode(type), TypeCode.SByte, TypeCode.Decimal);
		}

		public static bool IsPrimitive(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}
			return IsBetween(Type.GetTypeCode(type), TypeCode.Boolean, TypeCode.String);
		}

		public static bool IsNullable(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}
			return Nullable.GetUnderlyingType(type) != null;
		}

		public static string FriendlyTypeName(this Type type)
		{
			using (var provider = new CSharpCodeProvider())
			{
				var typeRef = new CodeTypeReference(type);
				string friendlyTypeName = provider.GetTypeOutput(typeRef);
				switch (friendlyTypeName)
				{
					case "System.DateTime":
						return "DateTime";
					case "System.String":
						return "string";
					default:
						return friendlyTypeName;
				}
			}
		}

		public static Type UnwrapIfNullable(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}
			return Nullable.GetUnderlyingType(type) ?? type;
		}

		public static Maybe<Type> GetElementTypeIfTypeImplementsEnumerableOfT(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			Type maybeFoundIEnumerable =
				new[] { type }.Concat(type.GetInterfaces())
					.FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
			return maybeFoundIEnumerable != null ? Maybe.Just(maybeFoundIEnumerable.GetGenericArguments()[0]) : Maybe.Nothing;
		}

		private static bool IsBetween(TypeCode typeCode, TypeCode left, TypeCode right)
		{
			return left <= typeCode && typeCode <= right;
		}

		public static bool IsDescendantOf(this Type son, Type maybeParent)
		{
			return maybeParent.IsAssignableFrom(son);
		}

		public static IEnumerable<Type> GetGenericTypeParameterOfInterface(this Type type, Type interfaceType)
		{
			return type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == interfaceType)
				.Select(x => x.GetGenericArguments()[0]);
		}


		public static IEnumerable<IGrouping<Type, TAttribute>> GetTypesWith<TAttribute>(bool inherit = false, params Assembly[] assembly)
			where TAttribute : Attribute
		{
			Assembly[] assemblies = assembly.Any()
				? assembly
				: AppDomain.CurrentDomain.GetAssemblies();

			return from a in assemblies
				from type in a.GetTypes()
				let attributes = Attribute.GetCustomAttributes(type).OfType<TAttribute>()
				where attributes.Any()
				from at in attributes
				group at by type;
		}

		public static bool IsDirectlyImplementsInterface(this Type type, Type @interface)
		{
			if (!@interface.IsInterface)
			{
				throw new InvalidOperationException();
			}
			if (type.IsInterface)
			{
				return false;
			}
			return type.GetInterfaces().Contains(@interface) && (type.BaseType == null || !type.BaseType.GetInterfaces().Contains(@interface));
		}

		public static IEnumerable<Type> GetBaseTypes(this Type type)
		{
			while (type != null)
			{
				yield return type;
				type = type.BaseType;
			}
		}

		/// <summary>
		/// Checks if type is a subclass of a generic base type
		/// Example: IsSubclassOfRawGeneric(typeof(Dog), typeof(Animal&lt;&gt;))
		/// </summary>
		public static bool IsSubclassOfRawGeneric(Type type, Type generic)
		{
			if (!generic.IsGenericTypeDefinition)
			{
				throw new ArgumentException(nameof(generic));
			}
			while (type != null && type != typeof(object))
			{
				var cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
				if (generic == cur)
				{
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}
	}
}

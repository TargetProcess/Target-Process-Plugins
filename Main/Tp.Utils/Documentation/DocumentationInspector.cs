using System;
using System.Diagnostics;
using System.Reflection;
using Tp.Core;
using Tp.Core.Annotations;

namespace Tp.Utils.Documentation
{
	public static class DocumentationInspector
	{
		/// <summary>
		/// Tries to detect a proper type to be documented from an attribute provider, e.g. from a custom type attribute on method parameters.
		/// Falls back to default type extraction if there is no info on the attribute level.
		/// </summary>
		[NotNull]
		public static Type ExtractDocumentationType([CanBeNull] ICustomAttributeProvider attributeProvider, [NotNull] Type valueType)
		{
			Debug.Assert(valueType != null, "valueType != null");

			return attributeProvider
				.NothingIfNull()
				.Bind(p => p.GetCustomAttribute<ApiTypeAttribute>())
				.Select(a => a.Type)
				.GetOrElse(() => ExtractDocumentationType(valueType));
		}

		/// <summary>
		/// Tries to detect a proper type to be documented.
		/// </summary>
		/// <param name="valueType">Original type of a property or a class to be documented.</param>
		[NotNull]
		public static Type ExtractDocumentationType([NotNull] Type valueType)
		{
			return valueType
				.GetInterfaces()
				.FirstOrNothing(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IApiType<>))
				.Select(i => ExtractDocumentationType(i.GetGenericArguments()[0]))
				.GetOrDefault(valueType);
		}
	}
}

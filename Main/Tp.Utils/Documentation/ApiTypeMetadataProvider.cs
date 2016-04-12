using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Tp.Utils.Documentation
{
	public class ApiTypeMetadataProvider
	{
		public ApiDataType GetTypeMetadata(Type type)
		{
			return type.IsEnum
				? GetForEnum(type)
				: GetForClass(type);
		}

		public ApiDataType GetTypeMetadata<T>()
		{
			return GetTypeMetadata(typeof(T));
		}

		private static ApiDataType GetForEnum(Type type)
		{
			Debug.Assert(type.IsEnum, "Should be an enumeration type");
			var fields = type
				.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where(f => !f.GetCustomAttribute<IgnoreApiDocumentationAttribute>().HasValue)
				.Select(f => new ApiParameter(f.Name, type, f.GetApiDescription()))
				.ToArray();

			return new ApiDataType(type, type.GetApiDescription(), fields);
		}

		private static ApiDataType GetForClass(Type type)
		{
			Debug.Assert(!type.IsEnum, "Should not be an enumeration type");
			var properties = type
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => !p.GetCustomAttribute<IgnoreApiDocumentationAttribute>().HasValue)
				.Select(p => new ApiParameter(p.Name, p.PropertyType, p.GetApiDescription()))
				.ToArray();

			return new ApiDataType(type, type.GetApiDescription(), properties);
		}
	}
}

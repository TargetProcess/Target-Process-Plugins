using System;

namespace Tp.Core
{
	public class EnumExtensions
	{
		public static Maybe<TAttribute> GetAttribute<TAttribute>(Enum enumValue)
			where TAttribute : class
		{
			var enumType = enumValue.GetType();
			return Enum.GetName(enumType, enumValue)
				.NothingIfNull()
				.Bind(valueName =>
				{
					var enumField = enumType.GetField(valueName);
					return Attribute.GetCustomAttribute(enumField, typeof(TAttribute)).NothingIfNull();
				})
				.OfType<TAttribute>();
		}
	}
}

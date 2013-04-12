using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tp.Core;

namespace Tp.Search.Model.Document
{
	class EnumServices
	{
		public static IDictionary<TEnum,TAttribute> Load<TEnum, TAttribute>()
			where TAttribute : Attribute
		{
			return Load<TEnum>(typeof (TAttribute)).ToDictionary(x => x.Key, x => (TAttribute) x.Value);
		}

		public static IDictionary<TEnum, Attribute> Load<TEnum>(Type attributeType) 
		{
			return typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static).Join(Enum.GetValues(typeof(TEnum)).Cast<TEnum>(), f => f.Name, e => e.ToString(), (f, e) => new { Field = f, EnumValue = e }).Select(
				p =>
				{
					var attribute = Attribute.GetCustomAttribute(p.Field, attributeType);
					return new
					{
						EnumAttribute = Maybe.ReturnIfNotNull(attribute),
						p.EnumValue
					};
				}).Where(x => x.EnumAttribute.HasValue)
					.ToDictionary(p => p.EnumValue, p => p.EnumAttribute.Value);
		}
	}
}
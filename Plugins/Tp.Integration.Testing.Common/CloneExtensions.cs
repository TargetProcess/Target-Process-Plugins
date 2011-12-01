// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Tp.Integration.Messages.EntityLifecycle;

namespace Tp.Integration.Testing.Common
{
	public static class CloneExtensions
	{
		public static T Clone<T>(this object obj) where T : class, new()
		{
			return obj.CreateMemento().RestoreMementoAs<T>();
		}
	}

	public static class MementoExtensions
	{
		public static PropertyDescriptor[] ConvertTo(this CustomPropertyDescriptorInfo[] memento, Func<CustomPropertyDescriptorInfo, PropertyDescriptor> convertor)
		{
			return memento.Select(convertor).ToArray();
		}


		public static CustomPropertyDescriptorInfo[] CreateMemento(this object instance)
		{
			var memento = new List<CustomPropertyDescriptorInfo>();

			var propertyInfos = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanRead && x.CanWrite);
			foreach (var propertyInfo in propertyInfos)
			{
				var customPropertyInfo = new CustomPropertyDescriptorInfo
				{
					Name = propertyInfo.Name,
					PropertyType = new DtoType(propertyInfo.PropertyType),
					Value = propertyInfo.GetValue(instance, null),
					AttributeArray = propertyInfo.GetCustomAttributes(true).Cast<Attribute>().ToArray()
				};
				memento.Add(customPropertyInfo);
			}
			return memento.ToArray();
		}

		public static object RestoreMementoAs(this CustomPropertyDescriptorInfo[] memento, Type type)
		{
			var instance = Activator.CreateInstance(type);

			foreach (var propertyInfo in memento)
			{
				var property = instance.GetType().GetProperty(propertyInfo.Name);
				property.SetValue(instance, propertyInfo.Value, null);
			}

			return instance;
		}

		public static T RestoreMementoAs<T>(this CustomPropertyDescriptorInfo[] memento)
			where T : class, new()
		{
			return (T)memento.RestoreMementoAs(typeof(T));
		}
	}

	public class CustomPropertyDescriptorInfo
	{
		public string Name { get; set; }
		public DtoType PropertyType { get; set; }
		public object Value { get; set; }
		public Attribute[] AttributeArray { get; set; }

		public CustomPropertyDescriptorInfo Clone()
		{
			return new CustomPropertyDescriptorInfo
			{
				Name = Name,
				PropertyType = PropertyType,
				AttributeArray = AttributeArray,
				Value = Value
			};
		}
	}
}
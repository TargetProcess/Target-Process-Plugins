// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Linq;

namespace Tp.Core.Dynamics
{
	[Serializable]
	public class Expando : DynamicObject, IDynamicMetaObjectProvider
	{
		private readonly Dictionary<string, PropertyInfo> _properties;
		public Expando()
		{
			_properties = this.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance).ToDictionary(x=>x.Name);
		}

		readonly Dictionary<string, object> _dynamicProperties = new Dictionary<string, object>();

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return _dynamicProperties.Keys;
		}


		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return GetMember(binder).TryGetValue(out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			Maybe<PropertyInfo> maybe = _properties.GetValue(binder.Name);

			if (maybe.HasValue)
			{
				maybe.Value.SetValue(this, value, null);
			}
			else
			{
				_dynamicProperties[binder.Name] = value;
			}
			return true;
		}

		private Maybe<object> GetMember(GetMemberBinder binder)
		{
			return _properties.GetValue(binder.Name)
				.Bind(property => property.GetValue(this, null))
				.OrElse(()=>_dynamicProperties.GetValue(binder.Name));
		}
	}
}
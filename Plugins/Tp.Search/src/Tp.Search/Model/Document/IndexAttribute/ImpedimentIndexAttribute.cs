// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;

namespace Tp.Search.Model.Document.IndexAttribute
{
	[AttributeUsage(AttributeTargets.Field)]
	class ImpedimentIndexAttribute : Attribute, IIndexFieldsProvider
	{
		private readonly IEnumerable<Enum> _fields;
		public ImpedimentIndexAttribute(ImpedimentField[] fields)
		{
			var customFields = CustomFieldsProvider.Instance.GetCustomFields<ImpedimentField>();
			_fields = fields.Cast<Enum>().Concat(customFields).ToArray();
		}

		public IEnumerable<Enum> IndexFields { get { return _fields; } }
	}
}
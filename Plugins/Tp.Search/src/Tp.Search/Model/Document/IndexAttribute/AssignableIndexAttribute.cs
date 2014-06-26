using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;

namespace Tp.Search.Model.Document.IndexAttribute
{
	[AttributeUsage(AttributeTargets.Field)]
	class AssignableIndexAttribute : Attribute, IIndexFieldsProvider
	{
		private readonly IEnumerable<Enum> _fields;
		public AssignableIndexAttribute(AssignableField[] fields)
		{
			var customFields = CustomFieldsProvider.Instance.GetCustomFields<AssignableField>();
			_fields = fields.Cast<Enum>().Concat(customFields).ToList();
		}

		public IEnumerable<Enum> IndexFields { get { return _fields; } }
	}
}
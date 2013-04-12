using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;

namespace Tp.Search.Model.Document.IndexAttribute
{
	[AttributeUsage(AttributeTargets.Field)]
	class TestCaseIndexAttribute : Attribute, IIndexFieldsProvider
	{
		private readonly IEnumerable<Enum> _fields;
		public TestCaseIndexAttribute(TestCaseField[] fields)
		{
			_fields = fields.Cast<Enum>().ToArray();
		}

		public IEnumerable<Enum> IndexFields { get { return _fields; } }
	}
}
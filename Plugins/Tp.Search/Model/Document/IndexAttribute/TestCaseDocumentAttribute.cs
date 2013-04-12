using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;

namespace Tp.Search.Model.Document.IndexAttribute
{
	[AttributeUsage(AttributeTargets.Field)]
	class TestCaseDocumentAttribute : Attribute, IDocumentFieldsProvider
	{
		private readonly IEnumerable<Enum> _fields;
		public TestCaseDocumentAttribute(TestCaseField[] fields)
		{
			_fields = fields.Cast<Enum>().ToList();
		}

		public IEnumerable<Enum> DocumentFields { get { return _fields; } }
	}
}
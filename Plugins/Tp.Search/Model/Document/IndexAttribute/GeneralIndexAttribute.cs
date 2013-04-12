using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;

namespace Tp.Search.Model.Document.IndexAttribute
{
	[AttributeUsage(AttributeTargets.Field)]
	class GeneralIndexAttribute : Attribute, IIndexFieldsProvider
	{
		private readonly Enum[] _fields;
		public GeneralIndexAttribute(GeneralField[] fields)
		{
			_fields = fields.Cast<Enum>().ToArray();
		}

		public IEnumerable<Enum> IndexFields { get { return _fields; } }
	}
}
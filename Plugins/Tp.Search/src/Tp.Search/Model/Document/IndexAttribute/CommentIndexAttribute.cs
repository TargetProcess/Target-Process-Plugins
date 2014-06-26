using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;

namespace Tp.Search.Model.Document.IndexAttribute
{
	[AttributeUsage(AttributeTargets.Field)]
	class CommentIndexAttribute : Attribute, IIndexFieldsProvider
	{
		private readonly IEnumerable<Enum> _fields;
		public CommentIndexAttribute(CommentField[] fields)
		{
			_fields = fields.Cast<Enum>().ToList();
		}

		public IEnumerable<Enum> IndexFields { get { return _fields; } }
	}
}
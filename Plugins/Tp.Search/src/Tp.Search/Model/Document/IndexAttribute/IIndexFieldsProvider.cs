using System;
using System.Collections.Generic;

namespace Tp.Search.Model.Document.IndexAttribute
{
	interface IIndexFieldsProvider
	{
		IEnumerable<Enum> IndexFields { get; }
	}
}
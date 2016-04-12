using System.Collections.Generic;

namespace System.Linq.Dynamic
{
	public interface ITypeProvider
	{
		IEnumerable<KeyValuePair<string, Type>> GetKnownTypes();
	}
}

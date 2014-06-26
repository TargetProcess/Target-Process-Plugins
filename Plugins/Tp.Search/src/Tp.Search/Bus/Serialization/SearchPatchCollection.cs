using System.Collections;
using System.Collections.Generic;
using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Search.Bus.Serialization
{
	internal class SearchPatchCollection : IPatchCollection
	{
		private readonly IEnumerable<IPatch> _pacthes;
		public SearchPatchCollection()
		{
			_pacthes = new List<IPatch>
				{
					new IndexExistingEntitiesSagaPreviousVersionCorrecter()
				};
		}
		public IEnumerator<IPatch> GetEnumerator()
		{
			return _pacthes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

using System.Collections.Generic;
using System.Linq;
using Tp.Search.Model.Document;
using hOOt;

namespace Tp.Search.Model.Query
{
	class QueryPlanResult
	{
		private readonly WAHBitArray _bitArray;
		private readonly IDictionary<DocumentIndexTypeToken, int> _versions;
		public QueryPlanResult(WAHBitArray bitArray, IEnumerable<KeyValuePair<DocumentIndexTypeToken,int>> versions)
		{
			_bitArray = bitArray;
			_versions = versions.ToDictionary(p => p.Key, p => p.Value);
		}

		public WAHBitArray BitArray
		{
			get { return _bitArray; }
		}

		public int GetVersion(DocumentIndexTypeToken token)
		{
			return _versions[token];
		}
	}
}
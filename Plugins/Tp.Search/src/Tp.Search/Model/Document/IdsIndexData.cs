using System;
using System.Collections.Generic;
using System.Linq;
using hOOt;

namespace Tp.Search.Model.Document
{
	public abstract class IdsIndexData
	{
		private readonly string _prefix;

		private readonly IEnumerable<int?> _ids;

		protected IdsIndexData(string prefix, IEnumerable<int?> ids)
		{
			_prefix = prefix;
			_ids = ids;
		}

		protected IEnumerable<int?> Ids
		{
			get { return _ids; }
		}

		public override string ToString()
		{
			return IndexDataStringServices.OfParts(Ids.Select(x => IndexDataStringServices.EncodeStringId(x, _prefix)));
		}

		protected static IEnumerable<int?> Sum<TD>(TD left, TD right) where TD : IdsIndexData
		{
			return left.Ids.Concat(right.Ids).Distinct().ToList();
		}

		protected static IEnumerable<int?> Substract<TD>(TD left, TD right) where TD : IdsIndexData
		{
			return left.Ids.Except(right.Ids).Distinct().ToList();
		}

		protected static IEnumerable<int?> Parse(string prefix, IndexData indexData)
		{
			return indexData.Words.Select(x => IndexDataStringServices.DecodeStringId(x, prefix)).ToList();
		}
	}
}
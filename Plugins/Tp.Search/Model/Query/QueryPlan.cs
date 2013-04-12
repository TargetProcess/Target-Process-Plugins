using System;
using Tp.Core;
using hOOt;

namespace Tp.Search.Model.Query
{
	struct QueryPlan
	{
		public Maybe<Lazy<WAHBitArray>> EntityPlan { get; set; }
		public Maybe<Lazy<WAHBitArray>> CommentPlan { get; set; }
	}
}
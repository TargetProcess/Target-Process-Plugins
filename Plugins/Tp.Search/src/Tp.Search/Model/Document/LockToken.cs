using System;

namespace Tp.Search.Model.Document
{
	struct LockToken
	{
		public IDisposable Token { get; set; }
		public DocumentIndexTypeToken TypeToken { get; set; }
		public int Version { get; set; }
	}
}
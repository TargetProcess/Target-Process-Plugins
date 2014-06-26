namespace Tp.Search.Model.Optimization
{
	class DocumentIndexOptimizeNoHint : IDocumentIndexOptimizeHint
	{
		public bool Advice()
		{
			return false;
		}
	}
}
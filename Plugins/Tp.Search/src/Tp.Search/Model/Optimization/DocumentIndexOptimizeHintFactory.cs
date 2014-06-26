using Tp.Search.Model.Document;

namespace Tp.Search.Model.Optimization
{
	class DocumentIndexOptimizeHintFactory
	{
		public IDocumentIndexOptimizeHint Create(DocumentIndexSetup setup)
		{
			switch (setup.DeferredOptimizeType)
			{
				case DeferredOptimizeType.None:
				case DeferredOptimizeType.Time:
					return new DocumentIndexOptimizeNoHint();
				case DeferredOptimizeType.Calls:
					return new DocumentIndexOptimizeByCallsHint(setup.DeferredOptimizeCounter);
				default:
					throw EnumServices.CreateUnexpectedEnumError(setup.DeferredOptimizeType);
			}
		}
	}
}

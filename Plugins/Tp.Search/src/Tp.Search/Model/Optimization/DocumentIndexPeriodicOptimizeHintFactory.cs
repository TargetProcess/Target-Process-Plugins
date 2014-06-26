using Tp.Search.Model.Document;

namespace Tp.Search.Model.Optimization
{
	class DocumentIndexPeriodicOptimizeHintFactory
	{
		public IDocumentIndexPeriodicOptimizeHint Create(DocumentIndexSetup setup)
		{
			switch (setup.DeferredOptimizeType)
			{
				case DeferredOptimizeType.Time:
					return new DocumentIndexPeriodicOptimizeHint();
				case DeferredOptimizeType.None:
				case DeferredOptimizeType.Calls:
					return new DocumentIndexPeriodicOptimizeNoHint();
				default:
					throw EnumServices.CreateUnexpectedEnumError(setup.DeferredOptimizeType);
			}
		}
	}
}
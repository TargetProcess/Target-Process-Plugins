namespace Tp.Search.Model.Document
{
	public class DocumentIndexOptimizeSetup
	{
		public static readonly DocumentIndexOptimizeSetup DeferredOptimize = new DocumentIndexOptimizeSetup(shouldOptimize:true,shouldDefer:true,shouldFreeMemory:true);
		public static readonly DocumentIndexOptimizeSetup ImmediateOptimize = new DocumentIndexOptimizeSetup(shouldOptimize: true, shouldDefer: false, shouldFreeMemory: true);
		public static readonly DocumentIndexOptimizeSetup NoOptimize = new DocumentIndexOptimizeSetup(shouldOptimize:false,shouldDefer:true,shouldFreeMemory:true);
		private readonly bool _shouldOptimize;
		private readonly bool _shouldDefer;
		private readonly bool _shouldFreeMemory;
		private DocumentIndexOptimizeSetup(bool shouldOptimize, bool shouldDefer, bool shouldFreeMemory)
		{
			_shouldOptimize = shouldOptimize;
			_shouldDefer = shouldDefer;
			_shouldFreeMemory = shouldFreeMemory;
		}

		public bool ShouldOptimize
		{
			get { return _shouldOptimize; }
		}

		public bool ShouldDefer
		{
			get { return _shouldDefer; }
		}

		public bool ShouldFreeMemory
		{
			get { return _shouldFreeMemory; }
		}
	}
}
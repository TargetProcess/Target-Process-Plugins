using Tp.Search.Model.Optimization;

namespace Tp.Search.Model.Document
{
	class DocumentIndexSetup
	{
		private readonly string _indexPath;
		private readonly int _minStringLengthToSearch;
		private readonly int _maxStringLengthIgnore;
		private readonly int _aliveTimeoutInMinutes;
		private readonly int _checkIntervalInMinutes;
		private readonly DeferredOptimizeType _deferredOptimizeType;
		private readonly int _deferredOptimizeCounter;
		private readonly int? _managedMemoryThresholdInMb;

		public DocumentIndexSetup(string indexPath, int minStringLengthToSearch, int maxStringLengthIgnore, int aliveTimeoutInMinutes, int checkIntervalInMinutes = 5, DeferredOptimizeType deferredOptimizeType = DeferredOptimizeType.Calls, int deferredOptimizeCounter = 100, int? managedMemoryThresholdInMb = null)
		{
			_indexPath = indexPath;
			_minStringLengthToSearch = minStringLengthToSearch;
			_maxStringLengthIgnore = maxStringLengthIgnore;
			_aliveTimeoutInMinutes = aliveTimeoutInMinutes;
			_checkIntervalInMinutes = checkIntervalInMinutes;
			_deferredOptimizeType = deferredOptimizeType;
			_deferredOptimizeCounter = deferredOptimizeCounter;
			_managedMemoryThresholdInMb = managedMemoryThresholdInMb;
		}

		public int MinStringLengthToSearch
		{
			get { return _minStringLengthToSearch; }
		}

		public int MaxStringLengthIgnore
		{
			get { return _maxStringLengthIgnore; }
		}

		public string IndexPath
		{
			get { return _indexPath; }
		}

		public int AliveTimeoutInMinutes
		{
			get { return _aliveTimeoutInMinutes; }
		}

		public int DeferredOptimizeCounter
		{
			get { return _deferredOptimizeCounter; }
		}

		public DeferredOptimizeType DeferredOptimizeType
		{
			get { return _deferredOptimizeType; }
		}

		public int CheckIntervalInMinutes
		{
			get { return _checkIntervalInMinutes; }
		}

		public int? ManagedMemoryThresholdInMb
		{
			get { return _managedMemoryThresholdInMb; }
		}
	}
}
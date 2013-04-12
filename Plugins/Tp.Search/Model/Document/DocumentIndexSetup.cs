namespace Tp.Search.Model.Document
{
	class DocumentIndexSetup
	{
		private readonly string _indexPath;
		private readonly int _minStringLengthToSearch;
		private readonly int _maxStringLengthIgnore;
		private readonly int _aliveTimeoutInMinutes;

		public DocumentIndexSetup(string indexPath, int minStringLengthToSearch, int maxStringLengthIgnore, int aliveTimeoutInMinutes)
		{
			_indexPath = indexPath;
			_minStringLengthToSearch = minStringLengthToSearch;
			_maxStringLengthIgnore = maxStringLengthIgnore;
			_aliveTimeoutInMinutes = aliveTimeoutInMinutes;
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
	}
}
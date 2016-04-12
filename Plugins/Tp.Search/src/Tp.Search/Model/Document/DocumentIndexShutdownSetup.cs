namespace Tp.Search.Model.Document
{
	struct DocumentIndexShutdownSetup
	{
		private readonly bool _forceShutdown;
		private readonly bool _cleanStorage;
		public DocumentIndexShutdownSetup(bool forceShutdown, bool cleanStorage)
		{
			_forceShutdown = forceShutdown;
			_cleanStorage = cleanStorage;
		}

		public bool ForceShutdown
		{
			get { return _forceShutdown; }
		}

		public bool CleanStorage
		{
			get { return _cleanStorage; }
		}
	}
}
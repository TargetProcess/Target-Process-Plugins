namespace Tp.MashupManager.MashupLibrary.Repository.Synchronizer
{
	public interface ILibraryRepositorySynchronizer
	{
		void BeginRead(ISynchronizableLibraryRepository repository);
		void EndRead(ISynchronizableLibraryRepository repository);
		bool TryBeginWrite(ISynchronizableLibraryRepository repository);
		void EndWrite(ISynchronizableLibraryRepository repository);
	}
}
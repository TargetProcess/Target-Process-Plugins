namespace Tp.MashupManager.MashupLibrary.Repository
{
	public interface ISynchronizableLibraryRepository : ILibraryRepository
	{
		string Id { get; }
	}
}
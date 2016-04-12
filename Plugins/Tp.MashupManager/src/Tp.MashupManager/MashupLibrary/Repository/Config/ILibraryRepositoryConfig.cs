namespace Tp.MashupManager.MashupLibrary.Repository.Config
{
	public interface ILibraryRepositoryConfig
	{
		string Name { get; }
		string Uri { get; }
		string Login { get; }
		string Password { get; }
	}
}
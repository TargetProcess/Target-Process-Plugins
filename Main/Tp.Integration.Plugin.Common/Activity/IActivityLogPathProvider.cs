namespace Tp.Integration.Plugin.Common.Activity
{
	public interface IActivityLogPathProvider
	{
		string GetLogPathFor(string accountName, string profileName, string fileName);

		string GetLogPathFor(string accountName, string profileName, string fileName, int chunk);

		string GetProfileLogDirectoryFor(string accountName, string profileName);

		string GetFileNameFrom(string fullName);

		string GetAccountNameFrom(string fileName);

		string GetProfileNameFrom(string fileName);
	}
}
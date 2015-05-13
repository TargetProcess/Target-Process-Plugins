namespace System
{
	public interface ITpUri
	{
		string ToAbsoluteUri(Uri uri);
		string ToAbsoluteUri();
		string ToString();
	}
}
namespace Tp.Core
{
	public interface INameValueDataProvider
	{
		object this[string name] { get; }
	}
}

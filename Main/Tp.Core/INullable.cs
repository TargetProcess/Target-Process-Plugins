namespace Tp.Core
{
	/// <summary>
	/// Base interface for reference type which has null object case
	/// </summary>
	public interface INullable
	{
		bool IsNull { get; }
	}
}

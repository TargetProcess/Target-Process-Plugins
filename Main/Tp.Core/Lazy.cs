// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class Lazy
	{
		public static Lazy<T> Create<T>(Func<T> valueFactory)
		{
			return new Lazy<T>(valueFactory);
		}
	}
}
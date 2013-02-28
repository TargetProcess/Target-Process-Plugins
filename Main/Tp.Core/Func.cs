namespace System
{
	public static class Func
	{
		public static Func<T,T> Identity<T>()
		{
			return _ => _;
		}
	}
}
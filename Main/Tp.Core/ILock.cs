namespace Tp.Core
{
	public interface ILock
	{
		void Acquire();
		void Release();
	}
}
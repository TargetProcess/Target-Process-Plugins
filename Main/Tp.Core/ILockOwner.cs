namespace Tp.Core
{
	public interface ILockOwner
	{
		bool IsLockTaken(ILock @lock);
	}
}
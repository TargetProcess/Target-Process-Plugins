namespace Tp.Core
{
	public interface ITransactionScopeProvider
	{
		Maybe<ILockOwner> RootTransactionScope { get; }
		ITransactionScope CreateTransactionScope(bool useBatchFlush = false, bool takeAccountLock = false);
	}
}
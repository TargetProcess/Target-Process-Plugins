using Tp.Core.Annotations;

namespace Tp.Core
{
	public interface ITransactionScopeProvider
	{
		Maybe<ILockOwner> RootTransactionScope { get; }

		[NotNull]
		ITransactionScope CreateTransactionScope(bool useBatchFlush = false, bool takeAccountLock = false);
	}
}
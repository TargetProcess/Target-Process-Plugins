using Tp.Core.Annotations;

namespace Tp.Core
{
	public interface ITransactionScopeProvider
	{
		Maybe<ILockOwner> RootTransactionScope { get; }

		/// <param name="useBatchFlush">if true session flushes will be postponed untill scope commit
		/// (direct Portal.Flush calls would flush anyway though)</param>
		/// <param name="takeAccountLock">if true takes pessimistic lock on whole account</param>
		/// <param name="forceFlushBeforeScopeStart">if true session will be flushed before scope creation</param>
		[NotNull]
		ITransactionScope CreateTransactionScope(bool useBatchFlush = false, bool takeAccountLock = false, bool forceFlushBeforeScopeStart = false);
	}
}

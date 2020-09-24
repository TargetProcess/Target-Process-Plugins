using Tp.Core.Annotations;

namespace Tp.Core
{
    public class TransactionScopeSetup
    {
        public static readonly TransactionScopeSetup BackwardCompatible = new TransactionScopeSetup(useBatchFlush:false, useAccountLock:false, forceFlushBeforeScopeStart:false, useSnapshotIsolationLevel:false, lockName:"");
        public static readonly TransactionScopeSetup Default = new TransactionScopeSetup(useBatchFlush:true, useAccountLock:false, forceFlushBeforeScopeStart:false, useSnapshotIsolationLevel:false, lockName:"");

        public TransactionScopeSetup(bool useBatchFlush = false, bool useAccountLock = false,
            bool forceFlushBeforeScopeStart = false, bool useSnapshotIsolationLevel = false, string lockName = "")
        {
            UseBatchFlush = useBatchFlush;
            UseAccountLock = useAccountLock;
            ForceFlushBeforeScopeStart = forceFlushBeforeScopeStart;
            UseSnapshotIsolationLevel = useSnapshotIsolationLevel;
            LockName = lockName;
        }

        /// <summary>
        /// if true session flushes will be postponed untill scope commit (direct Portal.Flush calls would flush anyway though)
        /// </summary>
        public bool UseBatchFlush {get;}
        /// <summary>
        /// if true takes pessimistic lock on whole account
        /// </summary>
        public bool UseAccountLock {get;}
        /// <summary>
        /// if true session will be flushed before scope creation
        /// </summary>
        public bool ForceFlushBeforeScopeStart {get;}
        /// <summary>
        ///(Optional) Custom namespace for account lock.
        ///Access is synchronized between account locks belonging to the same namespace.
        ///Actions in different lock namespaces are allowed to be executed simultaneously.
        ///
        ///This can be useful when you have several kinds of operations, each requiring a serialized execution of all operations of that kind,
        ///but the kinds themselves are not intersecting and you don't want to block one awaiting the completion of another.
        /// </summary>
        public string LockName { get; }
        /// <summary>
        ///if true db transaction isolation level is set to snapshot
        /// </summary>
        public bool UseSnapshotIsolationLevel {get;}
    }

    public static class TransactionScopeSetupFluent
    {
        public static TransactionScopeSetup Override(this TransactionScopeSetup from, bool? useBatchFlush = null, bool? useAccountLock = null, bool? forceFlushBeforeScopeStart = null, bool? useSnapshotIsolationLevel = null, string lockName = null)
        {
            return new TransactionScopeSetup(useBatchFlush ?? from.UseBatchFlush, useAccountLock ?? from.UseAccountLock, forceFlushBeforeScopeStart ?? from.ForceFlushBeforeScopeStart, useSnapshotIsolationLevel ??  from.UseSnapshotIsolationLevel, lockName ?? from.LockName);
        }
    }


    public interface ITransactionScopeProvider
    {
        Maybe<ILockOwner> RootTransactionScope { get; }

        [NotNull]
        ITransactionScope CreateTransactionScope([NotNull] TransactionScopeSetup setup);
    }

}

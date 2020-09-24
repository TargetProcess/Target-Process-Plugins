using System.Collections.Generic;
using System.Threading;
using Tp.Core;

namespace Tp.Integration.Messages.ServiceBus.Transport
{
    public class RetryCountPerMessageKeeper
    {
        private readonly ReaderWriterLockSlim _failuresPerMessageLocker = new ReaderWriterLockSlim();
        private readonly IDictionary<string, int> _failuresPerMessage = new Dictionary<string, int>();

        public int MaxRetries { get; set; } = 5;

        private bool RetryForever => MaxRetries == int.MaxValue;

        public bool HandledMaxRetries(string messageId)
        {
            if (RetryForever)
            {
                return false;
            }

            _failuresPerMessageLocker.EnterReadLock();
            var canRetry = !_failuresPerMessage.ContainsKey(messageId) ||
                _failuresPerMessage[messageId] < MaxRetries;
            _failuresPerMessageLocker.ExitReadLock();

            if (canRetry)
            {
                return false;
            }

            _failuresPerMessageLocker.EnterWriteLock();
            _failuresPerMessage.Remove(messageId);
            _failuresPerMessageLocker.ExitWriteLock();

            return true;
        }

        public int GetFailedCount(string messageId)
        {
            if (!_failuresPerMessage.ContainsKey(messageId))
            {
                return 0;
            }

            return _failuresPerMessage[messageId];
        }

        public void ClearFailuresForMessage(string messageId)
        {
            if (RetryForever)
            {
                return;
            }

            _failuresPerMessageLocker.EnterReadLock();
            var hasPerMessageFailure = _failuresPerMessage.ContainsKey(messageId);
            _failuresPerMessageLocker.ExitReadLock();

            if (!hasPerMessageFailure) return;

            _failuresPerMessageLocker.EnterWriteLock();
            _failuresPerMessage.Remove(messageId);
            _failuresPerMessageLocker.ExitWriteLock();
        }

        public void IncrementFailuresForMessage(string messageId)
        {
            if (RetryForever)
            {
                return;
            }

            _failuresPerMessageLocker.EnterWriteLock();
            try
            {
                if (!_failuresPerMessage.ContainsKey(messageId))
                {
                    _failuresPerMessage[messageId] = 1;
                }
                else
                {
                    _failuresPerMessage[messageId] += 1;
                }
            }
            finally
            {
                _failuresPerMessageLocker.ExitWriteLock();
            }
        }
    }
}

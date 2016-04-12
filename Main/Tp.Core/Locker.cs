using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Tp.Core
{
	public class Locker
	{
		private const int TIMEOUT = 30;
		private readonly ConcurrentDictionary<string, object> _registry = new ConcurrentDictionary<string, object>();

		private object GetLock(string fileName)
		{
			return _registry.GetOrAdd(fileName, new object());
		}

		/// <summary>
		///     Tries to execute action inside lock. Executes nothing if lock cannot be acquired.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="action"></param>
		/// <param name="timeout"></param>
		/// <returns>
		///     True if action executed successfully.
		///     False if lock cannot be acquired.
		/// </returns>
		public bool TryAcquireLockAndExecute(string key, Action action, int timeout = TIMEOUT)
		{
			return TryAcquireLockAndExecute(key,
				() =>
				{
					action();
					return true;
				},
				false,
				timeout);
		}

		/// <summary>
		///     Tries to execute func inside lock.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="func"></param>
		/// <param name="timeout"></param>
		/// <returns>
		///     Result of func invocation wrapped in Maybe.
		///     Maybe.Nothing if lock cannot be acquired.
		/// </returns>
		public Maybe<T> TryAcquireLockAndExecute<T>(string key, Func<T> func, int timeout = TIMEOUT)
		{
			return TryAcquireLockAndExecute<Maybe<T>>(key, () => func(), Maybe.Nothing, timeout);
		}

		private T TryAcquireLockAndExecute<T>(string key, Func<T> func, T valueIfLockCannotBeAcquired, int timeout)
		{
			var lockObj = GetLock(key);

			if (Monitor.TryEnter(lockObj, timeout))
			{
				using (Disposable.Create(() => Monitor.Exit(lockObj)))
				{
					return func();
				}
			}

			return valueIfLockCannotBeAcquired;
		}
	}
}

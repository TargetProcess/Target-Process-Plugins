using System;
using System.Diagnostics;

namespace Tp.Integration.Messages.ServiceBus.Transport.Router.Utils
{
	public class StopwatchObserver<TMessage> : IObserver<TMessage>
	{
		private readonly Stopwatch _runningStopwatch;
		private readonly Action<string> _log;
		private readonly Action<string> _logInfo;
		private readonly Action<Exception> _logOnError;
		private readonly Action _onComplete;
		private readonly Func<bool> _shouldStopwatchOnComplete;

		public StopwatchObserver(Stopwatch runningStopwatch, Action<string> log, Action<string> logInfo, Action<Exception> logOnError)
			: this(runningStopwatch, log, logInfo, logOnError, () => { }, () => true)
		{
		}

		public StopwatchObserver(Stopwatch runningStopwatch, Action<string> log, Action<string> logInfo, Action<Exception> logOnError, Action onComplete, Func<bool> shouldStopwatchOnComplete)
		{
			_runningStopwatch = runningStopwatch;
			_log = log;
			_logInfo = logInfo;
			_logOnError = logOnError;
			_onComplete = onComplete;
			_shouldStopwatchOnComplete = shouldStopwatchOnComplete;
		}

		public void OnNext(TMessage m)
		{
			_log("StopwatchObserver.OnNext");
		}

		public void OnError(Exception e)
		{
			_logOnError(e);
		}

		public void OnCompleted()
		{
			_onComplete();
			_logInfo("Observer completed");
			if (_shouldStopwatchOnComplete())
			{
				_runningStopwatch.Stop();
				_logInfo(string.Format("***********[Elapsed]***********: {0} ms", _runningStopwatch.Elapsed.TotalMilliseconds));
			}
		}
	}
}

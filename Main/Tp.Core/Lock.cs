using System.Threading;

namespace Tp.Core
{
	public class Lock : ILock
	{
		private readonly object _gate;

		public Lock(object gate)
		{
			_gate = gate;
		}

		public void Acquire()
		{
			Monitor.Enter(_gate);
		}

		public void Release()
		{
			Monitor.Exit(_gate);
		}
	}
}

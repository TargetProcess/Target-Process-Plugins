using System;
using System.Threading.Tasks;

namespace Tp.Core
{
	public interface ITaskFactory
	{
		Task StartNew(Action action);
	}

	public class TpTaskFactory : ITaskFactory
	{
		public Task StartNew(Action action)
		{
			return Task.Factory.StartNew(action);
		}
	}
}

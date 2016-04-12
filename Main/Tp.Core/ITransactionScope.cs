using System;

namespace Tp.Core
{
	public interface ITransactionScope : ILockOwner, IDisposable
	{
		void Commit();
	}
}

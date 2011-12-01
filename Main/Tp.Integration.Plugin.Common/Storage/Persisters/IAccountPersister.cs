using System.Collections.Generic;
using Tp.Integration.Messages;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
	public interface IAccountPersister
	{
		IList<Account> GetAll();
		Account Add(AccountName accountName);
		Account GetBy(AccountName accountName);
	}
}
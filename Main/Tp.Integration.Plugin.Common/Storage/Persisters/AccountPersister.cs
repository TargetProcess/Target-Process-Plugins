// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
	internal class AccountPersister : IAccountPersister
	{
		private readonly IDatabaseConfiguration _configuration;
		private readonly string _pluginName;

		public AccountPersister(IDatabaseConfiguration configuration, IPluginMetadata pluginMetadata)
		{
			_configuration = configuration;
			_pluginName = pluginMetadata.PluginData.Name;
		}

		public IList<Account> GetAll()
		{
			using (var context = CreateDataContext())
			{
				var dataLoadOptions = new DataLoadOptions();
				dataLoadOptions.LoadWith<Account>(a => a.Profiles);
				context.LoadOptions = dataLoadOptions;
				return context.Accounts.Where(x => x.Plugin.Name == _pluginName)
					.ToList()
					.AsReadOnly();
			}
		}

		public Account Add(AccountName accountName)
		{
			using (var context = CreateDataContext())
			{
				var newAccount = new Account {Name = accountName.Value};
				var plugin = context.Plugins.Single(x => x.Name == _pluginName);
				plugin.Accounts.Add(newAccount);
				context.SubmitChanges();
				return newAccount;
			}
		}

		public Account GetBy(AccountName accountName)
		{
			using (var context = CreateDataContext())
			{
				var dataLoadOptions = new DataLoadOptions();
				dataLoadOptions.LoadWith<Account>(a => a.Profiles);
				context.LoadOptions = dataLoadOptions;
				return context.Accounts.SingleOrDefault(x => x.Plugin.Name == _pluginName && x.Name == accountName.Value);
			}
		}

		public void Remove(AccountName accountName)
		{
			using (var context = CreateDataContext())
			{
				var dataLoadOptions = new DataLoadOptions();
				dataLoadOptions.LoadWith<Account>(a => a.Profiles);
				context.LoadOptions = dataLoadOptions;

				var account = context.Accounts.SingleOrDefault(x => x.Plugin.Name == _pluginName && x.Name == accountName.Value);
				if(account == null)
				{
					return;
				}
				context.Accounts.DeleteOnSubmit(account);
				context.SubmitChanges();
			}
		}

		private PluginDatabaseModelDataContext CreateDataContext()
		{
			return new PluginDatabaseModelDataContext(_configuration.ConnectionString);
		}
	}
}
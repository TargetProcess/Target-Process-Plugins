// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Integration.Plugin.Common.Storage.Persisters
{
	internal class ProfilePersister : IProfilePersister
	{
		private readonly IDatabaseConfiguration _configuration;
		private readonly string _pluginName;

		public ProfilePersister(IDatabaseConfiguration configuration, IPluginMetadata pluginMetadata)
		{
			_configuration = configuration;
			_pluginName = pluginMetadata.PluginData.Name;
		}

		private PluginDatabaseModelDataContext CreateDataContext()
		{
			return new PluginDatabaseModelDataContext(_configuration.ConnectionString);
		}

		public Profile Add(ProfileName profileName, bool initialized, AccountName accountName)
		{
			using (var context = CreateDataContext())
			{
				var profiles = SelectAccount(context.Accounts, accountName).Single().Profiles;
				var profileToSave = new Profile {Name = profileName.Value, Initialized = initialized};
				profiles.Add(profileToSave);
				context.SubmitChanges();
				return profileToSave;
			}
		}

		public Profile Update(ProfileName profileName, bool initialized, AccountName accountName)
		{
			using (var context = CreateDataContext())
			{
				var profileToUpdate = SelectProfile(context.Accounts, profileName, accountName);
				profileToUpdate.Initialized = initialized;
				context.SubmitChanges();
				return profileToUpdate;
			}
		}

		public void Delete(ProfileName profileName, AccountName accountName)
		{
			using (var context = CreateDataContext())
			{
				var profile = SelectProfile(context.Accounts, profileName, accountName);
				context.Profiles.DeleteOnSubmit(profile);
				context.SubmitChanges();
			}
		}

		public IEnumerable<Profile> GetAll(AccountName accountName)
		{
			using (var context = CreateDataContext())
			{
				return SelectAccount(context.Accounts, accountName).Single().Profiles;
			}
		}

		private IQueryable<Account> SelectAccount(IQueryable<Account> accounts, AccountName accountName)
		{
			return accounts.Where(a => a.Plugin.Name == _pluginName && a.Name == accountName.Value);
		}

		private Profile SelectProfile(IQueryable<Account> accounts, ProfileName profileName, AccountName accountName)
		{
			return SelectAccount(accounts, accountName).SelectMany(a => a.Profiles).Single(p => p.Name == profileName.Value);
		}
	}
}
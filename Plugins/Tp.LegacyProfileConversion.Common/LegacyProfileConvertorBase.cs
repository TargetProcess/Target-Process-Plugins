// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Messages.PluginLifecycle;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.LegacyProfileConvertsion.Common
{
	public abstract class LegacyProfileConvertorBase<TLegacyProfile>
	{
		private readonly IConvertorArgs _args;
		private readonly IAccountCollection _accountCollection;
		protected readonly TpDatabaseDataContext _context;

		protected LegacyProfileConvertorBase(IConvertorArgs args, IAccountCollection accountCollection)
		{
			_args = args;
			_accountCollection = accountCollection;
			_context = new TpDatabaseDataContext(args.TpConnectionString);
		}

		public void Execute()
		{
			var account = GetAccount();
			foreach (var legacyProfile in GetLegacyProfiles())
			{
				var pluginProfile = ConvertToPluginProfile(legacyProfile);
				FixName(pluginProfile);

				while (account.Profiles.Any(x => x.Name == pluginProfile.Name))
				{
					pluginProfile.Name = String.Format("{0} _re-converted_", pluginProfile.Name);
				}

				var profile = account.Profiles.Add(new ProfileCreationArgs(pluginProfile.Name, pluginProfile.Settings));
				OnProfileMigrated(profile);
				profile.MarkAsInitialized();
				profile.Save();
			}
		}

		private static void FixName(PluginProfileDto pluginProfile)
		{
			foreach (var profileNameChar in pluginProfile.Name.Distinct())
			{
				if (ProfileDtoValidator.IsValid(profileNameChar.ToString()))
				{
					continue;
				}
				pluginProfile.Name = pluginProfile.Name.Replace(profileNameChar, '_');
			}
		}

		private IAccount GetAccount()
		{
			return _accountCollection.GetOrCreate(_args.AccountName);
		}

		protected abstract void OnProfileMigrated(IStorageRepository storageRepository);
		protected abstract IEnumerable<TLegacyProfile> GetLegacyProfiles();
		protected abstract PluginProfileDto ConvertToPluginProfile(TLegacyProfile legacyProfile);

		protected TpUser GetUserBy(string fieldValue, Func<TpUser, string> getUserFieldValueToCompare)
		{
			return _context.TpUsers.ToArray().FirstOrDefault(x =>
			                                                 {
			                                                 	var fieldValueToCompare = getUserFieldValueToCompare(x) ??
			                                                 	                          string.Empty;

			                                                 	return fieldValueToCompare.ToLower() == fieldValue.ToLower() &&
			                                                 	       x.DeleteDate == null &&
			                                                 	       IsUserTypeCorrect(x);
			                                                 });
		}

		protected virtual bool IsUserTypeCorrect(TpUser user)
		{
			return true;
		}
	}
}
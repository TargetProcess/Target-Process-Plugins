// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
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
				ExecuteForProfile(legacyProfile, account);
			}
		}

		protected virtual void ExecuteForProfile(TLegacyProfile legacyProfile, IAccount account)
		{
			var pluginProfile = ConvertToPluginProfile(legacyProfile);
			FixName(pluginProfile);
		    CreateNewProfileName(pluginProfile, account);
			
			var profile = account.Profiles.Add(new ProfileCreationArgs(pluginProfile.Name, pluginProfile.Settings));
			OnProfileMigrated(profile, legacyProfile);
			profile.MarkAsInitialized();
			profile.Save();
		}

        protected virtual void CreateNewProfileName(PluginProfileDto pluginProfile, IAccount account)
        {
            while (account.Profiles.Any(x => x.Name == pluginProfile.Name))
            {
                pluginProfile.Name = String.Format("{0} _re-converted_", pluginProfile.Name);
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

		protected IAccount GetAccount()
		{
			return _accountCollection.GetOrCreate(_args.AccountName);
		}

		protected abstract void OnProfileMigrated(IStorageRepository storageRepository, TLegacyProfile legacyProfile);
		protected abstract IEnumerable<TLegacyProfile> GetLegacyProfiles();
		protected abstract PluginProfileDto ConvertToPluginProfile(TLegacyProfile legacyProfile);

		protected TpUser GetUserBy(string fieldValue, Func<TpUser, string> getUserFieldValueToCompare)
		{
			return _context.TpUsers.Where(u => u.Type == 1).ToArray()
				.FirstOrDefault(x =>CheckUserField(fieldValue, getUserFieldValueToCompare, x));
		}

		protected TpUser GetUserForProjectBy(int projectId, string fieldValue, Func<TpUser, string> getUserFieldValueToCompare)
		{
			var projectTeam = _context.ProjectMembers.Where(m => m.ProjectID == projectId).Select(m => m.UserID);

			return _context.TpUsers
				.Where(u => projectTeam.Contains(u.UserID))
				.Where(u => u.Type == 1)
				.ToArray()
				.FirstOrDefault(x => CheckUserField(fieldValue, getUserFieldValueToCompare, x));
		}

		private bool CheckUserField(string fieldValue, Func<TpUser, string> getUserFieldValueToCompare, TpUser x)
		{
			var fieldValueToCompare = getUserFieldValueToCompare(x) ??
			                          string.Empty;

			return fieldValueToCompare.ToLower() == fieldValue.ToLower() &&
			       x.DeleteDate == null &&
			       IsUserTypeCorrect(x);
		}

		protected Priority GetPriorityBy(string fieldValue, Func<Priority, string> getFieldToCompare)
		{
			return _context.Priorities.ToArray().FirstOrDefault(x => CompareEntities(fieldValue, getFieldToCompare, x));
		}

		protected Severity GetSeverityBy(string fieldValue, Func<Severity, string> getFieldToCompare)
		{
			return _context.Severities.ToArray().FirstOrDefault(x => CompareEntities(fieldValue, getFieldToCompare, x));
		}

		protected EntityState GetEntityStateBy(Process process, int entityTypeId, string fieldValue,
		                                       Func<EntityState, string> getFieldToCompare)
		{
			return
				_context.EntityStates.Where(s => s.ProcessID == process.ProcessID && s.EntityTypeID == entityTypeId).ToArray().
					FirstOrDefault(x => CompareEntities(fieldValue, getFieldToCompare, x));
		}

		protected Role GetRoleBy(string fieldValue, Func<Role, string> getFieldToCompare)
		{
			return _context.Roles.ToArray().FirstOrDefault(x => CompareEntities(fieldValue, getFieldToCompare, x));
		}

		private static bool CompareEntities<T>(string fieldValue, Func<T, string> predicate, T x)
		{
			var fieldValueToCompare = predicate(x) ?? string.Empty;
			return string.Equals(fieldValueToCompare, fieldValue,
			                     StringComparison.
			                     	InvariantCultureIgnoreCase);
		}

		protected virtual bool IsUserTypeCorrect(TpUser user)
		{
			return true;
		}
	}
}
// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Integration.Messages;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.PluginLifecycle.PluginCommand;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Commands
{
	public class AutomapVcsToTpUsersCommand : IPluginCommand
	{
		private readonly IPluginMetadata _pluginMetadata;
		private readonly IVersionControlSystemFactory _vcsFactory;

		public AutomapVcsToTpUsersCommand(IPluginMetadata pluginMetadata, IVersionControlSystemFactory vcsFactory)
		{
			_pluginMetadata = pluginMetadata;
			_vcsFactory = vcsFactory;
		}

		public PluginCommandResponseMessage Execute(string args)
		{
			var mappingArgs = args.Deserialize<AutomapVcsToTpUsersCommandArgs>();
			var alreadyMappedAuthors = mappingArgs.Connection.UserMapping.Select(x => x.Key);
			var vcs = _vcsFactory.Get(mappingArgs.Connection);
			var authors = vcs.RetrieveAuthors(new DateRange(CurrentDate.Value.AddMonths(-1), CurrentDate.Value));

			authors = authors
				.Where(x => alreadyMappedAuthors.FirstOrDefault(y => y.Trim().ToLower() == x.Trim().ToLower()) == null)
				.ToArray();

			var userLookups = new Dictionary<string, MappingLookup>();
			foreach (var author in authors)
			{
				var userLookup = GetMatchedUserLookup(author, mappingArgs.TpUsers);
				if (userLookup != null)
				{
					userLookups.Add(author, userLookup);
				}
			}

			var result = new AutomapVcsToTpUsersCommandResponse {UserLookups = userLookups};
			UpdateResultComment(result, authors);

			foreach (var mappingElement in mappingArgs.Connection.UserMapping)
			{
				result.UserLookups[mappingElement.Key] = mappingElement.Value;
			}

			return new PluginCommandResponseMessage {ResponseData = result.Serialize()};
		}

		private void UpdateResultComment(AutomapVcsToTpUsersCommandResponse result, IEnumerable<string> authors)
		{
			UpdateWithMappedSvnAuthors(result, authors);

			if (string.IsNullOrEmpty(result.Comment))
			{
				result.Comment += string.Format("No matches for {0} user(s) found", PluginName);
			}
		}

		private void UpdateWithMappedSvnAuthors(AutomapVcsToTpUsersCommandResponse result, IEnumerable<string> authors)
		{
			if (!result.UserLookups.Empty())
			{
				if (authors.Count() == result.UserLookups.Count)
				{
					result.Comment = string.Format("All the {0} {1} user(s) were mapped", result.UserLookups.Count, PluginName);
				}
				else
				{
					result.Comment = string.Format("{0} {1} user(s) were mapped", result.UserLookups.Count, PluginName);
				}

				UpdateWithUnmappedSvnAuthors(result, authors);
			}
		}

		private void UpdateWithUnmappedSvnAuthors(AutomapVcsToTpUsersCommandResponse result,
		                                          IEnumerable<string> authors)
		{
			var vcsUnmappedUsers = GetVcsUnmappedUsersCount(authors, result.UserLookups.Keys.ToArray());
			if (vcsUnmappedUsers != 0)
			{
				result.Comment += string.Format(", and no matches were found for {0} {1} user(s)", vcsUnmappedUsers, PluginName);
			}
		}

		private string PluginName
		{
			get { return _pluginMetadata.PluginData.Name; }
		}

		private static int GetVcsUnmappedUsersCount(IEnumerable<string> allSvnAuthors, IEnumerable<string> mappedSvnAuthors)
		{
			return allSvnAuthors.Except(mappedSvnAuthors).Count();
		}

		private static MappingLookup GetMatchedUserLookup(string author, List<TpUserMappingInfo> tpUsers)
		{
			var result = FindByEmail(author, tpUsers);
			if (result != null)
			{
				return new MappingLookup {Id = result.Id, Name = result.Name};
			}

			result = FindByName(author, tpUsers);
			if (result != null)
			{
				return new MappingLookup {Id = result.Id, Name = result.Name};
			}

			result = FindByLogin(author, tpUsers);

			return result != null ? new MappingLookup {Id = result.Id, Name = result.Name} : null;
		}

		private static TpUserMappingInfo FindByLogin(string author, IEnumerable<TpUserMappingInfo> tpUsers)
		{
			return (from user in tpUsers where user.Login.ToLower() == author.ToLower() select user).FirstOrDefault();
		}

		private static TpUserMappingInfo FindByName(string author, IEnumerable<TpUserMappingInfo> tpUsers)
		{
			return (from user in tpUsers where user.Name.ToLower() == author.ToLower() select user).FirstOrDefault();
		}

		private static TpUserMappingInfo FindByEmail(string author, IEnumerable<TpUserMappingInfo> tpUsers)
		{
			return (from user in tpUsers where user.Email.ToLower() == author.ToLower() select user).FirstOrDefault();
		}

		public string Name
		{
			get { return "Automap People"; }
		}
	}
}
// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;

namespace Tp.Tfs.Workflow
{
	public class TfsUserMapper : UserMapper
	{
		public TfsUserMapper(Func<IStorageRepository> storageRepository, Func<IActivityLogger> logger)
			: base(storageRepository, logger)
		{
		}

		protected override UserDTO GuessUser(RevisionInfo revision, IEnumerable<UserDTO> userDtos)
		{
			UserDTO result = null;

			if (AuthorNameIsSpecified(revision))
			{
				result = userDtos.FirstOrDefault(x => revision.Author.Equals(x.ActiveDirectoryName, StringComparison.OrdinalIgnoreCase));

				if (result == null)
				{
					var splittedLogin = revision.Author.Split('\\');

					var login = splittedLogin.Length > 1 ? splittedLogin[1] : splittedLogin[0];
					result = userDtos.FirstOrDefault(x => login.Equals(x.Login, StringComparison.OrdinalIgnoreCase));

					if (result != null)
						return result;

					result = userDtos.FirstOrDefault(x => login.Equals(x.FirstName + " " + x.LastName, StringComparison.OrdinalIgnoreCase));
				}
			}

			return result;
		}

		protected override bool AuthorIsSpecified(RevisionInfo revision)
		{
			return AuthorEmailIsSpecified(revision) || AuthorNameIsSpecified(revision);
		}

		private static bool AuthorNameIsSpecified(RevisionInfo revision)
		{
			return !string.IsNullOrEmpty(revision.Author);
		}

		private static bool AuthorEmailIsSpecified(RevisionInfo revision)
		{
			return !string.IsNullOrEmpty(revision.Email);
		}

		protected override MappingLookup GetTpUserFromMapping(RevisionInfo revision)
		{
			var userMapping = StorageRepository().GetProfile<TfsPluginProfile>().UserMapping;
			MappingLookup lookup = null;

			if (AuthorNameIsSpecified(revision))
			{
				lookup = userMapping[revision.Author];

				if (lookup != null)
					return lookup;

				var login = GetLogin(revision.Author);

				if (login != null)
					lookup = userMapping[login];
			}

			return lookup;
		}

		private string GetLogin(string domenLogin)
		{
			var splitted = domenLogin.Split(new[] { '\\' });

			return splitted.Length == 2 ? splitted[1] : null;
		}
	}
}
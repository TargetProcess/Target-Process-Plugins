// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.SourceControl.Settings;

namespace Tp.SourceControl
{
	public class CurrentProfileToConnectionSettingsAdapter<TProfile> : ISourceControlConnectionSettingsSource
		where TProfile : ConnectionSettings
	{
		protected IStorageRepository _repository;

		public CurrentProfileToConnectionSettingsAdapter(IStorageRepository repository)
		{
			_repository = repository;
		}

		public string Uri
		{
			get { return Profile.Uri; }
		}

		public string Login
		{
			get { return Profile.Login; }
		}

		public string Password
		{
			get { return Profile.Password; }
		}

		public string StartRevision
		{
			get { return Profile.StartRevision; }
			set { Profile.StartRevision = value; }
		}

		public MappingContainer UserMapping
		{
			get { return Profile.UserMapping; }
			set { Profile.UserMapping = value; }
		}

		private TProfile Profile
		{
			get { return _repository.GetProfile<TProfile>(); }
		}
	}
}
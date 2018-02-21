// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Diagnostics;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Events;
using Tp.Integration.Plugin.Common.Events.Aggregator;
using Tp.Integration.Plugin.Common.FileStorage;
using Tp.Integration.Plugin.Common.Storage.Repositories;

namespace Tp.Integration.Plugin.Common.Domain
{
    [DebuggerDisplay(
         "ProfileName = {_profileName.Value}, AccountName =  {_accountName.Value}, Initialized = {_initialized}, ProfileSettingsType = {_profileSettingsType}"
     )]
    class ProfileDomainObject : StorageRepositoryDelegate, IProfile
    {
        private readonly ProfileName _profileName;
        private readonly AccountName _accountName;
        private readonly Type _profileSettingsType;
        private object _settings;

        public ProfileDomainObject(ProfileName profileName, AccountName accountName, bool initialized, Type profileSettingsType)
        {
            _profileName = profileName;
            _accountName = accountName;
            _profileSettingsType = profileSettingsType;
            Initialized = initialized;
            _settings = null;
            EventAggregator = null;
            ProfileRepository = null;
        }

        public ProfileDomainObject(ProfileDomainObject other) : base(other)
        {
            _profileName = other._profileName;
            _accountName = other._accountName;
            _profileSettingsType = other._profileSettingsType;
            Initialized = other.Initialized;
            _settings = null;
            EventAggregator = other.EventAggregator;
            ProfileRepository = other.ProfileRepository;
        }

        public ProfileName Name
        {
            get { return _profileName; }
        }

        public void MarkAsInitialized()
        {
            Initialized = true;
        }

        public void MarkAsNotInitialized()
        {
            Initialized = false;
        }

        public bool Initialized { get; private set; }

        public object Settings
        {
            get { return _settings ?? (_settings = StorageRepository.GetProfile<object>()); }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(string.Format("Profile '{0}' settings should not be null", Name));
                }
                if (value.GetType() != _profileSettingsType)
                {
                    throw new ApplicationException(string.Format(
                        "Profile '{0}' settings has wrong '{1}' type. Valid settings type is '{2}'", Name, value.GetType(),
                        _profileSettingsType));
                }
                _settings = value;
            }
        }

        public IActivityLog Log
        {
            get { return new Log4NetActivityLog(_accountName.Value, _profileName.Value); }
        }

        public IProfileFileStorage FileStorage
        {
            get { return new ProfileFileStorage(_accountName.Value, _profileName.Value); }
        }

        public IEventAggregator EventAggregator { private get; set; }

        public IProfileRepository ProfileRepository { private get; set; }

        public void Save()
        {
            ProfileRepository.Update(this, _accountName);
            EventAggregator.Get<Event<ProfileChanged>>().Raise(new ProfileChanged(this, _accountName));
        }
    }
}

//  
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;

namespace Tp.SourceControl.Settings
{
    [DataContract]
    public class SourceControlSettings
    {
        [DataMember]
        public bool UsersMigrated { get; set; }
    }

    [DataContract, KnownType(typeof(ISourceControlConnectionSettingsSource))]
    public class ConnectionSettings : SourceControlSettings, ISourceControlConnectionSettingsSource
    {
        public const string UriField = "Uri";
        public const string LoginField = "Login";
        public const string PasswordField = "Password";

        private string _uri;

        [DataMember]
        public string Uri
        {
            get => _uri;
            set
            {
                if (value != null)
                {
                    _uri = value.Trim();
                }
            }
        }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        [SecretMember]
        public string Password { get; set; }

        private bool? _hasPassword;
        [DataMember]
        public bool HasPassword
        {
            get => _hasPassword ?? !Password.IsNullOrEmpty();
            set => _hasPassword = value;
        }

        [DataMember]
        public string StartRevision { get; set; }

        [DataMember]
        public MappingContainer UserMapping { get; set; }
    }
}

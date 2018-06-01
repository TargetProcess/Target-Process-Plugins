// 
// Copyright (c) 2005-2018 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Tp.Git.VersionControlSystem;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Plugin.Common.PluginCommand.Embedded;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Settings;

namespace Tp.Git
{
    [Profile]
    [DataContract]
    public class GitPluginProfile : ConnectionSettings, ISynchronizableProfile, IValidatable, IGitConnectionSettings
    {
        private const string SynchIntervalInMinutes = "SynchIntervalInMinutes";
        public const string StartRevisionField = "StartRevision";

        public GitPluginProfile()
        {
            UserMapping = new MappingContainer();
            StartRevision = GitRevisionId.UtcTimeMin.ToShortDateString();
        }

        private string _syncInterval;

        [DataMember]
        public string SyncInterval
        {
            get
            {
                if (!int.TryParse(_syncInterval, out var val))
                {
                    val = PluginSettings.LoadInt(SynchIntervalInMinutes, 0);
                }
                return Math.Max(MinimumSynchronizationIntervalInMinutes, val).ToString(CultureInfo.InvariantCulture);
            }
            set { _syncInterval = value; }
        }

        private const int MinimumSynchronizationIntervalInMinutes = 5;

        [IgnoreDataMember]
        public int SynchronizationInterval
        {
            get { return int.Parse(SyncInterval); }
            set { }
        }

        [DataMember]
        public bool UseSsh { get; set; }

        [DataMember]
        [SecretMember]
        public string SshPrivateKey { get; set; }

        private bool? _hasSshPrivateKey;
        [DataMember]
        public bool HasSshPrivateKey
        {
            get { return _hasSshPrivateKey ?? !SshPrivateKey.IsNullOrEmpty(); }
            set { _hasSshPrivateKey = value; }
        }

        [DataMember]
        public string SshPublicKey { get; set; }

        public bool ValidateStartRevision(PluginProfileErrorCollection errors)
        {
            return StartRevisionShouldBeValidDate(errors)
                && (StartRevisionShouldBeNotLessThanMin(errors) && StartRevisionShouldNotExceedTheMax(errors));
        }

        private bool StartRevisionShouldBeValidDate(PluginProfileErrorCollection errors)
        {
            if (!DateTime.TryParse(StartRevision, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal, out _))
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = StartRevisionField,
                    Message = "Start Revision Date should be specified in mm/dd/yyyy format."
                });
                return false;
            }
            return true;
        }

        private bool StartRevisionShouldNotExceedTheMax(PluginProfileErrorCollection errors)
        {
            DateTime result = DateTime.Parse(StartRevision, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
            if (result > GitRevisionId.UtcTimeMax)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = StartRevisionField,
                    Message = $"Start Revision Date should be not behind {GitRevisionId.UtcTimeMax.ToShortDateString()}."
                });
                return false;
            }
            return true;
        }

        private bool StartRevisionShouldBeNotLessThanMin(PluginProfileErrorCollection errors)
        {
            DateTime result = DateTime.Parse(StartRevision, CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
            if (result < GitRevisionId.UtcTimeMin)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = StartRevisionField,
                    Message = $"Start Revision Date should be not before {GitRevisionId.UtcTimeMin.ToShortDateString()}."
                });
                return false;
            }
            return true;
        }

        public void Validate(PluginProfileErrorCollection errors)
        {
            ValidateUri(errors);
            ValidateStartRevision(errors);
            ValidateUserMapping(errors);
            ValidateSshKeys(errors);
        }

        public void ValidateSshKeys(PluginProfileErrorCollection errors)
        {
            if (!UseSsh)
            {
                return;
            }
            if (SshPrivateKey.IsNullOrEmpty())
            {
                errors.Add(new PluginProfileError { Message = "Please specify SSH private key.", FieldName = nameof(SshPrivateKey) });
            }

            if (!SshPrivateKey.StartsWith("-----BEGIN RSA PRIVATE KEY-----"))
            {
                errors.Add(new PluginProfileError
                {
                    Message = "SSH private key has incorrect format. It should begin with `-----BEGIN RSA PRIVATE KEY-----`.",
                    FieldName = nameof(SshPrivateKey)
                });
            }

            if (!ContainsOnlyValidSymbols(SshPrivateKey))
            {
                errors.Add(new PluginProfileError
                {
                    Message = "SSH private key contains ivalid symbols.",
                    FieldName = nameof(SshPrivateKey)
                });
            }

            if (SshPublicKey.IsNullOrEmpty())
            {
                errors.Add(new PluginProfileError { Message = "Please specify SSH public key.", FieldName = nameof(SshPublicKey) });
            }

            if (!ContainsOnlyValidSymbols(SshPublicKey))
            {
                errors.Add(new PluginProfileError
                {
                    Message = "SSH public key contains ivalid symbols.",
                    FieldName = nameof(SshPublicKey)
                });
            }

            bool ContainsOnlyValidSymbols(string key)
            {
                return key.Select(c => (int) c).All(c => c == 10 || c == 13 || (c > 31 && c < 126));
            }
        }

        public void ValidateUri(PluginProfileErrorCollection errors)
        {
            ValidateUriIsNotEmpty(errors);
            ValidateUriFormat(errors);
        }

        private void ValidateUriFormat(PluginProfileErrorCollection errors)
        {
            if (!string.IsNullOrEmpty(Uri) && !IsCommonUri() && !IsGitUri() && !IsFileUri() && !IsSshUri())
            {
                errors.Add(new PluginProfileError { Message = "Wrong Uri format.", FieldName = UriField });
            }
        }

        private bool IsSshUri()
        {
            return Regex.IsMatch(Uri, @"^ssh://(.+@)?([\w\d\.]+)(:\d+)?/(~\S+)?(/\S)*$", RegexOptions.IgnoreCase)
                || Regex.IsMatch(Uri,
                    @"^(?!file:)(?!http:)(?!https:)(?!ftp:)(?!ftps:)(?!rsync:)(?!git:)(?!ssh:)(.+@)?[\w\d\-._~%]+[\.][\w\d]+:(/~.+/)?\S*$",
                    RegexOptions.IgnoreCase);
        }

        private bool IsGitUri()
        {
            return Regex.IsMatch(Uri, @"^git://([\w\d\.-]+)(:\d+)?/(~(\S+))?(\S*)?$", RegexOptions.IgnoreCase);
        }

        private bool IsFileUri()
        {
            return Regex.IsMatch(Uri, @"^file:///\S+$", RegexOptions.IgnoreCase);
        }

        private bool IsCommonUri()
        {
            return Regex.IsMatch(Uri, @"^((http|https|ftp|ftps|rsync):)?//(.*@)?[\w\d\.~-]+(:\d+)?(/[\S]*)?$", RegexOptions.IgnoreCase);
        }

        private void ValidateUriIsNotEmpty(PluginProfileErrorCollection errors)
        {
            if (string.IsNullOrEmpty(Uri))
            {
                errors.Add(new PluginProfileError { FieldName = UriField, Message = "Uri should not be empty." });
            }
        }

        private void ValidateUserMapping(PluginProfileErrorCollection errors)
        {
            if (UserMapping.Select(x => x.Key.ToLower()).Distinct().Count() != UserMapping.Count)
            {
                errors.Add(new PluginProfileError
                {
                    FieldName = "user-mapping",
                    Message = "Can't map an svn user to TargetProcess user twice."
                });
            }
        }
    }
}

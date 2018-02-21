using System.Collections.Generic;
using Microsoft.Win32;
using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;

namespace Tp.Git.VersionControlSystem
{
    public class NGitMockSystemReader : SystemReader
    {
        private sealed class MockConfig : FileBasedConfig
        {
            public MockConfig(FilePath cfgLocation, FS fs) : base(cfgLocation, fs)
            {
            }

            /// <exception cref="System.IO.IOException"></exception>
            /// <exception cref="NGit.Errors.ConfigInvalidException"></exception>
            public override void Load()
            {
            }

            // Do nothing
            public override bool IsOutdated()
            {
                return false;
            }
        }

        private readonly IDictionary<string, string> _values = new Dictionary<string, string>();
        private readonly FileBasedConfig _userGitConfig;
        private readonly FileBasedConfig _systemGitConfig;

        public static void Register()
        {
            SystemReader.SetInstance(new NGitMockSystemReader(SystemReader.GetInstance()));
        }

        public NGitMockSystemReader(SystemReader initialReader)
        {
            Init(Constants.OS_USER_NAME_KEY);
            Init(Constants.GIT_AUTHOR_NAME_KEY);
            Init(Constants.GIT_AUTHOR_EMAIL_KEY);
            Init(Constants.GIT_COMMITTER_NAME_KEY);
            Init(Constants.GIT_COMMITTER_EMAIL_KEY);
            Init("os.name",
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion")?
                    .GetValue("ProductName")
                    .ToString() ?? "Windows");
            _userGitConfig = new MockConfig(null, null);
            _systemGitConfig = new MockConfig(null, null);
            _initialSystemReader = initialReader;
        }

        private void Init(string n, string value = null)
        {
            SetProperty(n, value ?? n);
        }

        public virtual void ClearProperties()
        {
            _values.Clear();
        }

        public virtual void SetProperty(string key, string value)
        {
            _values.Put(key, value);
        }

        public override string Getenv(string variable)
        {
            return _values.Get(variable);
        }

        public override string GetProperty(string key)
        {
            return _values.Get(key);
        }

        public override FileBasedConfig OpenUserConfig(Config parent, FS fs)
        {
            return _userGitConfig;
        }

        public override FileBasedConfig OpenSystemConfig(Config parent, FS fs)
        {
            return _systemGitConfig;
        }

        public override string GetHostname()
        {
            return "fake.host.example.com";
        }

        public override long GetCurrentTime()
        {
            return _initialSystemReader.GetCurrentTime();
        }

        private readonly SystemReader _initialSystemReader;
        // Sat Aug 15 20:12:58 GMT-03:30 2009
        public override int GetTimezone(long when)
        {
            return _initialSystemReader.GetTimezone(when);
        }
    }
}
// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Microsoft.Win32;
using NGit;
using NGit.Storage.File;
using NGit.Util;
using Sharpen;
using System.Collections.Generic;

namespace Tp.MashupManager.MashupLibrary.Repository
{
    public class MockSystemReader : SystemReader
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

        private readonly SystemReader _initialSystemReader;
        private readonly IDictionary<string, string> _values = new Dictionary<string, string>();
        private readonly FileBasedConfig _userGitConfig;
        private readonly FileBasedConfig _systemGitConfig;

        public MockSystemReader(SystemReader initialReader)
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
            SetProperty("jgit.fs.debug", false.ToString()); //bug #72646. The value is required in case of Cygwin is installed
            _userGitConfig = new MockConfig(null, null);
            _systemGitConfig = new MockConfig(null, null);
            _initialSystemReader = initialReader;
        }

        private void Init(string n, string value = null)
        {
            SetProperty(n, value ?? n);
        }

        public void ClearProperties()
        {
            _values.Clear();
        }

        public void SetProperty(string key, string value)
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

        public override FileBasedConfig OpenUserConfig(NGit.Config parent, FS fs)
        {
            return _userGitConfig;
        }

        public override FileBasedConfig OpenSystemConfig(NGit.Config parent, FS fs)
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

        // Sat Aug 15 20:12:58 GMT-03:30 2009
        public override int GetTimezone(long when)
        {
            return _initialSystemReader.GetTimezone(when);
        }
    }

    static class DictionaryExtensions
    {
        public static U Get<T, U>(this IDictionary<T, U> d, T key)
        {
            U val;
            d.TryGetValue(key, out val);
            return val;
        }

        public static U Put<T, U>(this IDictionary<T, U> d, T key, U value)
        {
            U old;
            d.TryGetValue(key, out old);
            d[key] = value;
            return old;
        }
    }
}

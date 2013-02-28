using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Tp.SourceControl.Testing
{
    public class ConfigHelper
    {
        private static ConfigHelper _instance;
        private static object _syncObject = new object();
        
        private ConfigHelper()
        {
        }

        public static ConfigHelper Instance
        {
            get
            {
                lock (_syncObject)
                {
                    if (_instance == null)
                        _instance = new ConfigHelper();

                    return _instance;    
                }
            }
        }

        public string TestCollectionWithMergeCommit
        {
            get
            {
                return GetAppSettingsValue("TestCollectionWithMergeCommit", "http://localhost:8080/tfs/TestRepositoryWithMergeCommit");
            }
        }

        public string TestProjectWithMergeCommit
        {
            get
            {
                return GetAppSettingsValue("TestProjectWithMergeCommit", "TestRepositoryWithMergeCommit");
            }
        }

        public string TestCollectionWithFileDeleted
        {
            get
            {
                return GetAppSettingsValue("TestCollectionWithFileDeleted", "http://localhost:8080/tfs/TestCollectionWithFileDeleted");
            }
        }

        public string TestProjectWithFileDeleted
        {
            get
            {
                return GetAppSettingsValue("TestProjectWithFileDeleted", "TestRepositoryWithFileDeleted");
            }
        }

        public string FuncTestCollection
        {
            get
            {
                return GetAppSettingsValue("FuncTestCollection", "http://localhost:8080/tfs/functestscollection");
            }
        }

        public string FuncTestsProject
        {
            get
            {
                return GetAppSettingsValue("FuncTestsProject", "FuncTestsProject");
            }
        }

        public string TestCollection
        {
            get
            {
                return GetAppSettingsValue("TestCollection", "http://localhost:8080/tfs/testcollection");
            }
        }

        public string Domen
        {
            get
            {
                return GetAppSettingsValue("Domen", string.Empty);
            }
        }

        public string Login
        {
            get
            {
                return GetAppSettingsValue("Login", "{0}\\operator".Fmt(Environment.MachineName));
            }
        }

        public string Password
        {
            get
            {
                return GetAppSettingsValue("Password", "trustMIND");
            }
        }

        public string TestCollectionProject
        {
            get
            {
                return GetAppSettingsValue("TestCollectionProject", "TeamProject1");
            }
        }

        private string GetAppSettingsValue(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
    }
}

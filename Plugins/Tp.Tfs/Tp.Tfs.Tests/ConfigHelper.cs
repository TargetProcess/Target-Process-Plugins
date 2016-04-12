using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Tp.Tfs.Tests
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

        public string TestCollection
        {
            get
            {
                return GetAppSettingsValue("TestCollection", "http://localhost:8080/tfs/testcollection");
            }
        }

        public string Login
        {
            get
            {
                return GetAppSettingsValue("Login", "operator");
            }
        }

        public string Domen
        {
            get
            {
                return GetAppSettingsValue("Domen", string.Empty);
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

        public int RevisionFrom
        {
            get
            {
                return GetAppSettingsIntValue("RevisionFrom", 1);
            }
        }

        public int RevisionTill
        {
            get
            {
                return GetAppSettingsIntValue("RevisionTill", 200);
            }
        }

        public int PageSize
        {
            get
            {
                return GetAppSettingsIntValue("PageSize", 100);
            }
        }

        private int GetAppSettingsIntValue(string key, int defaultValue)
        {
            string value = GetAppSettingsValue(key, defaultValue.ToString());

            int revisionFrom;
            if (int.TryParse(value, out revisionFrom))
                return revisionFrom;

            return defaultValue;
        }

        private string GetAppSettingsValue(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }


    }
}

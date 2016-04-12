using System;
using System.Configuration;

namespace Tp.SourceControl.Testing
{
	public class ConfigHelper
	{
		private static ConfigHelper _instance;
		private static readonly object SyncObject = new object();

		private ConfigHelper()
		{
		}

		public static ConfigHelper Instance
		{
			get
			{
				lock (SyncObject)
				{
					return _instance ?? (_instance = new ConfigHelper());
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
				return "{0}\\{1}".Fmt(Environment.MachineName, GetAppSettingsValue("Login", "operator"));
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

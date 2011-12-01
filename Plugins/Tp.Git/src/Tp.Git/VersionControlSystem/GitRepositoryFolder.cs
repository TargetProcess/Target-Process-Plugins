// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Reflection;
using NGit.Storage.File;

namespace Tp.Git.VersionControlSystem
{
	[Serializable]
	public class GitRepositoryFolder
	{
		public string Value { get; set; }
		public string RepoUri { get; set; }

		public void Delete()
		{
			if (!Exists())
			{
				return;
			}

			ShutdownGit(this);
			DeleteDirectory();
		}

		public bool Exists()
		{
			return Directory.Exists(Value);
		}

		private static void ShutdownGit(GitRepositoryFolder repositoryFolder)
		{
			var git = NGit.Api.Git.Open(repositoryFolder.Value);
			git.GetRepository().Close();
			WindowCache.Reconfigure(new WindowCacheConfig());
		}

		private void DeleteDirectory()
		{
			Value.DeleteDirectory();
		}

		public static GitRepositoryFolder Create(string repoUri)
		{
			string gitRootFolderAbsolutePath = GitCloneRootFolder;
			if (string.IsNullOrEmpty(GitCloneRootFolder))
			{
				gitRootFolderAbsolutePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);
			}

			return new GitRepositoryFolder {Value = Path.Combine(gitRootFolderAbsolutePath, Guid.NewGuid().ToString()), RepoUri = repoUri};
		}

		protected static string GitCloneRootFolder
		{
			get
			{
				var customPluginConfig = ConfigurationSettings.GetConfig("customPluginSettings") as Hashtable;
				if (customPluginConfig != null)
				{
					var repoRootFolder = customPluginConfig["GitRepoRootFolder"];
					if (repoRootFolder != null)
					{
						return repoRootFolder.ToString();
					}
				}

				return ".";
			}
		}
	}
}
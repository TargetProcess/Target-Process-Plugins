// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using NGit.Storage.File;
using StructureMap;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Git.VersionControlSystem
{
	[Serializable]
	public class GitRepositoryFolder
	{
		public string Value { get; set; }
		public string RepoUri { get; set; }

		[NonSerialized]
		private bool _wasMarkedAsDeleted;

		public void Delete()
		{
			if (!Exists())
			{
				return;
			}

			try
			{
				ShutdownGit(this);
			}
			catch (Exception ex)
			{
				ObjectFactory.GetInstance<IActivityLogger>().Error(ex);
			}

			try
			{
				DeleteDirectory();
			}
			catch (Exception ex)
			{
				_wasMarkedAsDeleted = true;
				ObjectFactory.GetInstance<IActivityLogger>().Error(ex);
			}
		}

		public bool Exists()
		{
			return Directory.Exists(Value) && !_wasMarkedAsDeleted;
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
			return new GitRepositoryFolder {Value = Path.Combine(GitCloneRootFolder, Guid.NewGuid().ToString()), RepoUri = repoUri};
		}

		protected static string GitCloneRootFolder
		{
			get { return ObjectFactory.GetInstance<PluginDataFolder>().Path; }
		}
	}
}
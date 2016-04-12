// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tp.SourceControl.Settings;

namespace Tp.Subversion.Subversion
{
	public static class LocalRepositorySettings
	{
		/// <summary>
		/// Specify your user name.
		/// </summary>
		private const string USERNAME = "test";

		/// <summary>
		/// Specify your password.
		/// </summary>
		private const string PASSWORD = "123456";

		public static Uri GetLocalRepositoryUri(string relativePath)
		{
			return new Uri(string.Format("file:///{0}", Path.Combine(GetExecutingDirectory(), relativePath)));
		}

		public static string GetExecutingDirectory()
		{
			var fileName = new Uri(typeof (LocalRepositorySettings).Assembly.CodeBase).AbsolutePath;
			return Path.GetDirectoryName(fileName);
		}

		public static ConnectionSettings Create(string repoPath)
		{
			return Create(repoPath, string.Empty);
		}

		public static ConnectionSettings Create(string repoPath, string relativePath)
		{
			return new ConnectionSettings
			{
				Uri = GetLocalRepositoryUri(Path.Combine(repoPath, relativePath)).ToString(),
				Login = USERNAME,
				Password = PASSWORD
			};
		}
	}
}
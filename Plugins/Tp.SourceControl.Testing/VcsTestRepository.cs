// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using Tp.SourceControl.Testing.Repository;
using Tp.SourceControl.Testing.Repository.Svn;

namespace Tp.SourceControl.Testing
{
	public abstract class VcsTestRepository<TActualVcsRepository> : IVcsRepository
	{
		protected VcsTestRepository()
		{
			Deploy();
			OnTestRepositoryDeployed();
		}

		protected string LocalRepositoryPath
		{
			get { return Path.Combine(GetExecutingDirectory(), Name); }
		}

		protected abstract string Name { get; }

		public Uri Uri
		{
			get { return new Uri(String.Format("file:///{0}", LocalRepositoryPath)); }
		}

		protected static string GetExecutingDirectory()
		{
			var fileName = new Uri(typeof (SvnTestRepository).Assembly.CodeBase).AbsolutePath;
			return Path.GetDirectoryName(fileName);
		}

		private void Deploy()
		{
			LocalRepositoryPath.DeleteDirectory();

			var assembly = Assembly.GetExecutingAssembly();
			using (var stream = assembly.GetManifestResourceStream(typeof (TActualVcsRepository), RepositoryResourceName))
			{
				var buffer = new byte[stream.Length];
				string repositoryZipFilePath = Path.Combine(GetExecutingDirectory(), RepositoryResourceName);
				using (var tempFile = new FileStream(repositoryZipFilePath, FileMode.Create))
				{
					stream.Read(buffer, 0, (int) stream.Length);
					tempFile.Write(buffer, 0, buffer.Length);
				}

				var fastZip = new FastZip {CreateEmptyDirectories = true};
				fastZip.ExtractZip(repositoryZipFilePath, GetExecutingDirectory(), string.Empty);
			}
		}

		private string RepositoryResourceName
		{
			get { return Name + ".zip"; }
		}

		public abstract string Login { get; }
		public abstract string Password { get; }
		public abstract void Commit(string commitComment);
		public abstract string Commit(string filePath, string changedContent, string commitComment);
		public abstract void CheckoutBranch(string branch);
		public abstract string CherryPick(string revisionId);

		protected virtual void OnTestRepositoryDeployed() {}
	}
}
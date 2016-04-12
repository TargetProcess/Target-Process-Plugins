// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NGit.Api;
using NGit.Api.Errors;
using NGit.Transport;
using Tp.Integration.Messages;
using Tp.MashupManager.MashupLibrary.Package;
using Tp.MashupManager.MashupLibrary.Repository.Config;
using Tp.MashupManager.MashupLibrary.Repository.Exceptions;
using Tp.MashupManager.MashupLibrary.Repository.Synchronizer;

namespace Tp.MashupManager.MashupLibrary.Repository
{
	public class LibraryRepository : ISynchronizableLibraryRepository
	{
		private const string GitFolderName = ".git";
		private const string ReadmeMarkdownFileName = "README.mkd";
		private const string BaseInfoFileTemplate = "*.baseinfo.json";
		private const string InitializationMarkFileName = "repository.initialized";
		private readonly ILibraryRepositoryConfig _config;

		private readonly string _initializationMarkFilePath;
		private readonly IMashupLoader _mashupLoader;
		private readonly string _path;
		private readonly ILibraryRepositorySynchronizer _synchonizer;

		public LibraryRepository(ILibraryRepositoryConfig config, ILibraryLocalFolder folder,
		                         ILibraryRepositorySynchronizer synchronizer, IMashupLoader mashupLoader)
		{
			_config = config;
			_path = Path.Combine(folder.Path, config.Name);
			_initializationMarkFilePath = Path.Combine(_path, InitializationMarkFileName);
			_synchonizer = synchronizer;
			_mashupLoader = mashupLoader;
		}

		public void Refresh()
		{
			if (_synchonizer.TryBeginWrite(this))
			{
				try
				{
					if (Exists())
					{
						Pull();
					}
					else
					{
						Clone();
					}

					Initialized = true;
				}
				finally
				{
					_synchonizer.EndWrite(this);
				}
			}
		}

		public IEnumerable<LibraryPackage> GetPackages()
		{
			if (!Initialized)
			{
				Refresh();
			}

			_synchonizer.BeginRead(this);
			try
			{
				return Directory
					.EnumerateDirectories(_path)
					.Select(p => new {FullPath = p, FolderName = new DirectoryInfo(p).Name})
					.Where(p => !p.FolderName.Equals(GitFolderName, StringComparison.InvariantCultureIgnoreCase))
					.Select(p => new LibraryPackage
						{
							Name = p.FolderName,
							BaseInfo = GetBaseInfo(p.FullPath)
						}
					);
			}
			finally
			{
				_synchonizer.EndRead(this);
			}
		}

		public Mashup GetPackageMashup(string packageName)
		{
			_synchonizer.BeginRead(this);
			try
			{
				return _mashupLoader.Load(Path.Combine(_path, packageName), packageName);
			}
			finally
			{
				_synchonizer.EndRead(this);	
			}			
		}

		public LibraryPackageDetailed GetPackageDetailed(string packageName)
		{
			_synchonizer.BeginRead(this);
			try
			{
				var packageFullPath = Path.Combine(_path, packageName);
				if (!Directory.Exists(packageFullPath))
				{
					throw new PackageNotFoundException(packageName);
				}

				return new LibraryPackageDetailed
					{
						Name = packageName,
						BaseInfo = GetBaseInfo(packageFullPath),
						ReadmeMarkdown = GetReadmeMarkdown(packageFullPath)
					};
			}
			finally
			{
				_synchonizer.EndRead(this);
			}
		}

		private CredentialsProvider CredentialsProvider
		{
			get
			{
				return string.IsNullOrEmpty(_config.Login)
					       ? CredentialsProvider.GetDefault()
					       : new UsernamePasswordCredentialsProvider(_config.Login, _config.Password);
			}
		}

		public string Id
		{
			get { return _config.Name; }
		}

		private bool Initialized
		{
			get { return File.Exists(_initializationMarkFilePath); }

			set
			{
				if (value && !File.Exists(_initializationMarkFilePath))
				{
					File.Create(_initializationMarkFilePath).Dispose();
				}
			}
		}

		private bool Exists()
		{
			return Directory.Exists(_path);
		}

		private void Pull()
		{
			Git git = Git.Open(_path);
			try
			{
				git.Fetch().SetCredentialsProvider(CredentialsProvider).Call();
				git.Reset().SetMode(ResetCommand.ResetType.HARD).SetRef("origin/master").Call();
				git.GetRepository().Close();
			}
			catch (InvalidRemoteException)
			{
				git.GetRepository().Close();
				Remove();
				Clone();
			}
			catch(Exception)
			{
				git.GetRepository().Close();
				throw;
			}
		}

		private void Clone()
		{
			Git git = Git
				.CloneRepository()
				.SetURI(_config.Uri)
				.SetCredentialsProvider(CredentialsProvider)
				.SetDirectory(_path)
				.Call();
			git.GetRepository().Close();
		}

		private void Remove()
		{
			var pathDi = new DirectoryInfo(_path);
			pathDi.GetFiles("*", SearchOption.AllDirectories).ForEach(fi => fi.Attributes = FileAttributes.Normal);
			pathDi.Delete(true);
		}

		private LibraryPackageBaseInfo GetBaseInfo(string mashupFolderPath)
		{
			string baseInfoFile =
				Directory.EnumerateFiles(mashupFolderPath, BaseInfoFileTemplate, SearchOption.AllDirectories).FirstOrDefault();
			return baseInfoFile != null
				       ? File.ReadAllText(baseInfoFile).Deserialize<LibraryPackageBaseInfo>()
				       : new LibraryPackageBaseInfo
					       {
						       ShortDescription = "",
						       CompatibleTpVersion = new LibraryPackageCompatibleTpVersion {Minimum = ""}
					       };
		}

		private string GetReadmeMarkdown(string packageFullPath)
		{
			var readmeMarkdownFullPath = Path.Combine(packageFullPath, ReadmeMarkdownFileName);
			return File.Exists(readmeMarkdownFullPath)
				       ? File.ReadAllText(readmeMarkdownFullPath)
				       : string.Empty;
		}
	}
}
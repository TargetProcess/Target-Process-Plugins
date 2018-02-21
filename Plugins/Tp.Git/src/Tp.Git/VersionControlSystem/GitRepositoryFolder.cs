// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Reflection;
using NGit.Storage.File;
using StructureMap;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Git.VersionControlSystem
{
    [Serializable]
    public class GitRepositoryFolder
    {
        public string Value { get; set; }
        public string RepoUri { get; set; }

        [NonSerialized] private bool _wasMarkedAsDeleted;

        public void Delete()
        {
            if (!Exists())
            {
                return;
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
            string path = GetAbsolutePath();
            bool exists = Directory.Exists(path);
            return exists && !_wasMarkedAsDeleted;
        }

        private void DeleteDirectory()
        {
            string path = GetAbsolutePath();
            path.DeleteDirectory();
        }

        public static GitRepositoryFolder Create(string repoUri)
        {
            GitRepositoryFolder folder = new GitRepositoryFolder();
            folder.RepoUri = repoUri;

            // form relative path to local repository
            Uri absoluteUri = new Uri(Path.Combine(GitCloneRootFolder, Guid.NewGuid().ToString()));
            Uri baseUri = new Uri(AppDomain.CurrentDomain.BaseDirectory);
            Uri relativeUri = baseUri.MakeRelativeUri(absoluteUri);
            folder.Value = relativeUri.OriginalString;

            return folder;
        }

        private string GetRepositoryFolder()
        {
            Uri uri = new Uri(GetAbsolutePath());
            string[] segments = uri.Segments;
            string repository = segments[segments.Length - 1];
            return repository;
        }

        private bool CheckNewCloneRepository(IStorage<GitRepositoryFolder> storage)
        {
            string repository = GetRepositoryFolder();
            string clonePath = Path.Combine(GitCloneRootFolder, repository);

            Uri absoluteUri = new Uri(clonePath);
            Uri baseUri = new Uri(AppDomain.CurrentDomain.BaseDirectory);
            Uri relativeUri = baseUri.MakeRelativeUri(absoluteUri);

            Assembly assembly = Assembly.GetExecutingAssembly();
            Uri uri = new Uri(assembly.CodeBase);
            string path = Path.Combine(Path.GetDirectoryName(uri.AbsolutePath), relativeUri.OriginalString);

            if (Directory.Exists(path))
            {
                Value = relativeUri.OriginalString;
                storage.ReplaceWith(this);
                return true;
            }

            return false;
        }

        public bool CheckFolder(IStorage<GitRepositoryFolder> storage)
        {
            if (!Directory.Exists(GetAbsolutePath()) ||
                string.Compare(GetAbsolutePath(), Value, true) == 0)
            {
                bool newExists = CheckNewCloneRepository(storage);
                return newExists;
            }

            return true;
        }

        public string GetAbsolutePath()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Uri uri = new Uri(assembly.CodeBase);
            string path = Path.Combine(Path.GetDirectoryName(uri.AbsolutePath), Value);
            return path;
        }

        protected static string GitCloneRootFolder
        {
            get { return ObjectFactory.GetInstance<PluginDataFolder>().Path; }
        }
    }
}

// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Reflection;
using StructureMap;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using MercurialSDK = Mercurial;

namespace Tp.Mercurial.VersionControlSystem
{
    [Serializable]
    public class MercurialRepositoryFolder
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

        public static MercurialRepositoryFolder Create(string repoUri)
        {
            MercurialRepositoryFolder folder = new MercurialRepositoryFolder();
            folder.RepoUri = repoUri;

            // form relative path to local repository
            Uri absoluteUri = new Uri(Path.Combine(MercurialCloneRootFolder, Guid.NewGuid().ToString()));
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

        private bool CheckNewCloneRepository(IStorage<MercurialRepositoryFolder> storage)
        {
            string repository = GetRepositoryFolder();
            string clonePath = Path.Combine(MercurialCloneRootFolder, repository);

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

        public bool CheckFolder(IStorage<MercurialRepositoryFolder> storage)
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

        protected static string MercurialCloneRootFolder
        {
            get { return ObjectFactory.GetInstance<PluginDataFolder>().Path; }
        }
    }
}

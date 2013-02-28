// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using StructureMap;

namespace Tp.SourceControl.Testing.Repository.Tfs
{
	public class TfsTestRepository : IVcsRepository
	{
	    private Workspace _workspace;

        public TfsTestRepository()
		{
            ObjectFactory.Configure(x => x.For<TfsTestRepository>().HybridHttpOrThreadLocalScoped().Use(this));

            Deploy();
		}

        public void Dispose()
        {
            if (_workspace != null)
            {
                _workspace.Delete();
                _workspace = null;
            }
        }
        
		protected string Name
		{
			get { return "TestRepository"; }
		}

        private string ClonedRepoFolder
        {
            get { return LocalRepositoryPath + "Cloned"; }
        }

        protected string LocalRepositoryPath
        {
            get { return Path.Combine(GetExecutingDirectory(), Name); }
        }

        protected static string GetExecutingDirectory()
        {
            var fileName = new Uri(typeof(TfsTestRepository).Assembly.CodeBase).AbsolutePath;
            return Path.GetDirectoryName(fileName);
        }

        public string GetLatestRevision()
        {
            TfsTeamProjectCollection collection = new TfsTeamProjectCollection(new Uri(ConfigHelper.Instance.TestCollection));
            var vcs = collection.GetService<VersionControlServer>();
            TeamProject tp = vcs.GetTeamProject(ConfigHelper.Instance.TestCollectionProject);
            var maxChangeset = vcs.QueryHistory(
                        tp.ServerItem,
                        VersionSpec.Latest,
                        0,
                        RecursionType.Full,
                        null,
                        null,
                        null,
                        Int32.MaxValue,
                        true,
                        true).Cast<Changeset>().Max(x => x.ChangesetId);

            collection.Dispose();

            return maxChangeset.ToString();
        }

        private void Deploy()
        {
            TfsTeamProjectCollection collection;

            // if setting "Domen" in config file initialized - it means that test run on the local machine, 
            // otherwise means that test run on the specialized testing machine
            if (string.IsNullOrEmpty(ConfigHelper.Instance.Domen))
                collection = new TfsTeamProjectCollection(new Uri(ConfigHelper.Instance.TestCollection));
            else
                collection = new TfsTeamProjectCollection(
                    new Uri(ConfigHelper.Instance.TestCollection),
                    new NetworkCredential(ConfigHelper.Instance.Login, ConfigHelper.Instance.Password, ConfigHelper.Instance.Domen));

            var vcs = collection.GetService<VersionControlServer>();
            TeamProject tp = vcs.GetTeamProject(ConfigHelper.Instance.TestCollectionProject);

            string workspaceName = "MyWorkspace";
            string projectPath = tp.ServerItem;
            string workingDirectory = ClonedRepoFolder;

            if (Directory.Exists(workingDirectory))
            {
                var files = Directory.GetFiles(workingDirectory, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                    File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);

                Directory.Delete(workingDirectory, true);
            }
                
            Directory.CreateDirectory(workingDirectory);

            Workspace[] workspaces = vcs.QueryWorkspaces(workspaceName, vcs.AuthorizedUser, Workstation.Current.Name);
            if (workspaces.Length > 0)
                vcs.DeleteWorkspace(workspaceName, vcs.AuthorizedUser);

            _workspace = vcs.CreateWorkspace(workspaceName, vcs.AuthorizedUser, "Test Workspace");

            try
            {
                _workspace.Map(projectPath, workingDirectory);
                GetRequest request = new GetRequest(new ItemSpec(projectPath, RecursionType.Full), VersionSpec.Latest);
                GetStatus status = _workspace.Get(request, GetOptions.GetAll | GetOptions.Overwrite); 
            }
            catch
            {}
            
        }

	    public Uri Uri
	    {
	        get
	        {
                return new Uri(string.Concat(ConfigHelper.Instance.TestCollection, "/", ConfigHelper.Instance.TestCollectionProject));
	        }
	    }

	    public string Login
		{
			get { return ConfigHelper.Instance.Login; }
		}

		public string Password
		{
			get { return ConfigHelper.Instance.Password; }
		}

		public void Commit(string commitComment)
		{
			Commit("secondFile.txt", "my changed content", commitComment);
		}

		public string Commit(string serverItemPath, string changedContent, string commitComment)
		{
            TfsTeamProjectCollection collection = new TfsTeamProjectCollection(new Uri(ConfigHelper.Instance.TestCollection));
            var vcs = collection.GetService<VersionControlServer>();
            TeamProject tp = vcs.GetTeamProject(ConfigHelper.Instance.TestCollectionProject);
		    ItemSet itemSet = vcs.GetItems(tp.ServerItem, VersionSpec.Latest, RecursionType.Full, DeletedState.NonDeleted, ItemType.File);
            Item item = itemSet.Items.FirstOrDefault(x => x.ServerItem == serverItemPath);

            string localItem = _workspace.GetLocalItemForServerItem(item.ServerItem);
            int changesetId = _workspace.PendEdit(localItem);

            using (var file = File.OpenWrite(localItem))
            {
                var changes = new UTF8Encoding(true).GetBytes(changedContent);
                file.Seek(0, SeekOrigin.End);
                file.Write(changes, 0, changes.Length);
            }

            PendingChange[] pendingChanges = _workspace.GetPendingChanges().Where(x => x.ChangeType == ChangeType.Edit).ToArray();
            int changeset = _workspace.CheckIn(pendingChanges, ConfigHelper.Instance.Login, commitComment, null, null, null);

            Changeset latestChangeset = vcs.GetChangeset(changeset);

            collection.Dispose();

            return latestChangeset.ChangesetId.ToString();
		}

        public void CheckoutBranch(string branch)
        {
            throw new NotImplementedException();
        }

        public string CherryPick(string revisionId)
        {
            throw new NotImplementedException();
        }
	}
}
// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using StructureMap;

namespace Tp.SourceControl.Testing.Repository.Tfs
{
    public class TfsFuncTestRepository : IVcsRepository
    {
        private Workspace _workspace;

        public TfsFuncTestRepository()
        {
            ObjectFactory.Configure(x => x.For<TfsFuncTestRepository>().HybridHttpOrThreadLocalScoped().Use(this));

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
            get { return "FuncTestRepository"; }
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

        public string GetLastButOneRevision()
        {
            TfsTeamProjectCollection collection = new TfsTeamProjectCollection(new Uri(ConfigHelper.Instance.FuncTestCollection));
            var vcs = collection.GetService<VersionControlServer>();
            TeamProject tp = vcs.GetTeamProject(ConfigHelper.Instance.FuncTestsProject);

            var changesets = vcs.QueryHistory(
                tp.ServerItem,
                VersionSpec.Latest,
                0,
                RecursionType.Full,
                null,
                null,
                null,
                Int32.MaxValue,
                true,
                true).Cast<Changeset>().ToArray();

            collection.Dispose();

            if (changesets.Count() == 1)
                return changesets.First().ChangesetId.ToString();

            int lastButOneChangeset = changesets.Where(x => x.ChangesetId < changesets.Max(m => m.ChangesetId)).Max(x => x.ChangesetId);

            return lastButOneChangeset.ToString();
        }

        private void Deploy()
        {
            TfsTeamProjectCollection collection = new TfsTeamProjectCollection(new Uri(ConfigHelper.Instance.FuncTestCollection));
            var vcs = collection.GetService<VersionControlServer>();
            TeamProject tp = vcs.GetTeamProject(ConfigHelper.Instance.FuncTestsProject);

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
            { }

        }

        public Uri Uri
        {
            get
            {
                return new Uri(ConfigHelper.Instance.FuncTestCollection + "/" + ConfigHelper.Instance.FuncTestsProject);
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
            Commit(string.Concat("$/", ConfigHelper.Instance.FuncTestsProject + "/testFile.txt"), "my changed content", commitComment);
        }

        public string Commit(string serverItemPath, string changedContent, string commitComment)
        {
            TfsTeamProjectCollection collection = new TfsTeamProjectCollection(new Uri(ConfigHelper.Instance.FuncTestCollection));
            var vcs = collection.GetService<VersionControlServer>();
            TeamProject tp = vcs.GetTeamProject(ConfigHelper.Instance.FuncTestsProject);
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

        public void ClearFileContent()
        {
            TfsTeamProjectCollection collection = new TfsTeamProjectCollection(new Uri(ConfigHelper.Instance.FuncTestCollection));
            var vcs = collection.GetService<VersionControlServer>();
            TeamProject tp = vcs.GetTeamProject(ConfigHelper.Instance.FuncTestsProject);
            ItemSet itemSet = vcs.GetItems(tp.ServerItem, VersionSpec.Latest, RecursionType.Full, DeletedState.NonDeleted, ItemType.File);
            Item item = itemSet.Items.FirstOrDefault(x => x.ServerItem == string.Concat("$/", ConfigHelper.Instance.FuncTestsProject + "/testFile.txt"));

            string localItem = _workspace.GetLocalItemForServerItem(item.ServerItem);
            _workspace.PendEdit(localItem);

            using (var file = File.Open(localItem, FileMode.Truncate)) { }

            PendingChange[] pendingChanges = _workspace.GetPendingChanges().Where(x => x.ChangeType == ChangeType.Edit).ToArray();
			_workspace.CheckIn(pendingChanges, ConfigHelper.Instance.Login, string.Empty, null, null, null);
            
            collection.Dispose();
        }
    }
}
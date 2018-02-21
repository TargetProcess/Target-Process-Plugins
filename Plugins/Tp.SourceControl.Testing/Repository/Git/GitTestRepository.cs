// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NGit;
using StructureMap;

namespace Tp.SourceControl.Testing.Repository.Git
{
    internal static class Extensions
    {
        public static readonly long EpochTicks;

        static Extensions()
        {
            EpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        }

        public static long ToMillisecondsSinceEpoch(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("dateTime is expected to be expressed as a UTC DateTime", nameof(dateTime));
            }
            return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), TimeSpan.Zero).ToMillisecondsSinceEpoch();
        }

        public static long ToMillisecondsSinceEpoch(this DateTimeOffset dateTimeOffset)
        {
            return (dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks - EpochTicks) / TimeSpan.TicksPerMillisecond;
        }
    }

    public class GitTestRepository : VcsTestRepository<GitTestRepository>
    {
        public GitTestRepository()
        {
            ObjectFactory.Configure(x => x.For<GitTestRepository>().HybridHttpOrThreadLocalScoped().Use(this));
        }

        private NGit.Api.Git _git;

        private static int _testCount;

        protected override void OnTestRepositoryDeployed()
        {
            base.OnTestRepositoryDeployed();

            EnsureUserHomePathForSystemUser();

            var clonedRepoFolder = CreateUniqueTestFolderPrefix();

            _git = NGit.Api.Git.CloneRepository()
                .SetURI(LocalRepositoryPath)
                .SetDirectory(clonedRepoFolder).Call();
        }

        private static string CreateUniqueTestFolderPrefix()
        {
            return Path.Combine(GetExecutingDirectory(), "trash", $"{DateTime.UtcNow.ToMillisecondsSinceEpoch()}_{_testCount++}");
        }

        protected override string Name => "TestRepository";

        public override string Login => "test";

        public override string Password => "test";

        public override void Commit(string commitComment)
        {
            Commit("secondFile.txt", "my changed content", commitComment);
        }

        public override string Commit(string filePath, string changedContent, string commitComment)
        {
            using (var file = File.OpenWrite(Path.Combine(_git.GetRepository().WorkTree.ToString(), filePath)))
            {
                var changes = new UTF8Encoding(true).GetBytes(changedContent);
                file.Write(changes, 0, changes.Length);
            }

            _git.Add().AddFilepattern(filePath).Call();
            var commit = _git.Commit().SetMessage(commitComment).SetAuthor(Login, "admin@admin.com").Call();
            _git.Push().Call();

            return commit.Id.Name;
        }

        public override void CheckoutBranch(string branch)
        {
            var fullBranchName = "refs/heads/" + branch;
            var list = _git.BranchList().Call();
            var branchExists = list.Any(x => x.GetName() == fullBranchName);
            if (!branchExists)
            {
                var remoteBranchName = "refs/remotes/origin/" + branch;
                _git.BranchCreate().SetStartPoint(remoteBranchName).SetName(branch).Call();
            }
            _git.Checkout().SetName(branch).Call();
        }

        public override string CherryPick(string revisionId)
        {
            var result = _git.CherryPick().Include(ObjectId.FromString(revisionId)).Call();
            _git.Push().Call();

            return result.GetCherryPickedRefs().Select(x => x.GetName()).First();
        }

        private void EnsureUserHomePathForSystemUser()
        {
            using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
            {
                if (!identity.IsSystem) return;
                var type = typeof(Sharpen.FilePath).Assembly.GetTypes().FirstOrDefault(ta => ta.Name == "Runtime");
                if (type != null)
                {
                    var method = type.GetMethod("GetProperties", BindingFlags.Public | BindingFlags.Static);
                    method.Invoke(null, null);
                    var field = type.GetField("properties", BindingFlags.NonPublic | BindingFlags.Static);
                    if (field != null)
                    {
                        var properties = (System.Collections.Hashtable) field.GetValue(null);
                        if (string.IsNullOrEmpty(properties["user.home"] as string))
                        {
                            properties["user.home"] = Path.Combine(Path.Combine(GetExecutingDirectory(), "App_Data"), "git");
                        }
                    }
                }
            }
        }
    }
}

// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Linq;
using System.Text;
using Mercurial;
using StructureMap;
using MercurialSDK = Mercurial;

namespace Tp.SourceControl.Testing.Repository.Mercurial
{
    public class MercurialTestRepository : VcsTestRepository<MercurialTestRepository>
    {
        public MercurialTestRepository()
        {
            ObjectFactory.Configure(x => x.For<MercurialTestRepository>().HybridHttpOrThreadLocalScoped().Use(this));
        }

        private MercurialSDK.Repository _repository;

        private string ClonedRepoFolder => LocalRepositoryPath + "Cloned";

        protected override void OnTestRepositoryDeployed()
        {
            base.OnTestRepositoryDeployed();

            if (Directory.Exists(ClonedRepoFolder))
                Directory.Delete(ClonedRepoFolder, true);

            Directory.CreateDirectory(ClonedRepoFolder);

            _repository = new MercurialSDK.Repository(ClonedRepoFolder);
            _repository.Clone(LocalRepositoryPath, new CloneCommand());
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
            using (var file = File.OpenWrite(Path.Combine(ClonedRepoFolder, filePath)))
            {
                var changes = new UTF8Encoding(true).GetBytes(changedContent);
                file.Write(changes, 0, changes.Length);
            }

            _repository.Add(new AddCommand().WithPaths(filePath));
            _repository.Commit(commitComment, new CommitCommand().WithOverrideAuthor("test"));
            _repository.Push();

            var lastChangeset = _repository.Log().First();

            return lastChangeset.Revision;
        }

        public override void CheckoutBranch(string branch)
        {
            throw new NotImplementedException();
        }

        public override string CherryPick(string revisionId)
        {
            throw new NotImplementedException();
        }
    }
}

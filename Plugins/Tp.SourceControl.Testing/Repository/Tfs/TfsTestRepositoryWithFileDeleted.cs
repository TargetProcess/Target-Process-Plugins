// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.SourceControl.Testing.Repository.Tfs
{
    public class TfsTestRepositoryWithFileDeleted : IVcsRepository
    {
        protected string Name
        {
            get { return "TestRepositoryWithFileDeleted"; }
        }

        public Uri Uri
        {
            get
            {
                return
                    new Uri(string.Concat(ConfigHelper.Instance.TestCollectionWithFileDeleted, "/",
                        ConfigHelper.Instance.TestProjectWithFileDeleted));
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
            throw new System.NotImplementedException();
        }

        public string Commit(string filePath, string changedContent, string commitComment)
        {
            throw new System.NotImplementedException();
        }

        public void CheckoutBranch(string branch)
        {
            throw new System.NotImplementedException();
        }

        public string CherryPick(string revisionId)
        {
            throw new System.NotImplementedException();
        }
    }
}

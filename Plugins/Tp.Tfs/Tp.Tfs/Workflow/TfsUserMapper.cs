// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.SourceControl;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;

namespace Tp.Tfs.Workflow
{
    public class TfsUserMapper : UserMapper
    {
        public TfsUserMapper(Func<IStorageRepository> storageRepository, Func<IActivityLogger> logger)
            : base(storageRepository, logger)
        {
        }

        protected override TpUserData GuessUser(RevisionInfo revision, ICollection<TpUserData> userDtos)
        {
            TpUserData result = null;

            if (AuthorNameIsSpecified(revision))
            {
                result = userDtos.FirstOrDefault(x => revision.Author.Equals(x.ActiveDirectoryName, StringComparison.OrdinalIgnoreCase));

                if (result == null)
                {
                    var login = GetDomainLogin(revision.Author) ?? revision.Author;
                    result = userDtos.FirstOrDefault(x => login.Equals(x.Login, StringComparison.OrdinalIgnoreCase));

                    if (result != null)
                        return result;
                }
            }

            return result;
        }

        protected override bool AuthorIsSpecified(RevisionInfo revision)
        {
            return AuthorEmailIsSpecified(revision) || AuthorNameIsSpecified(revision);
        }

        private static bool AuthorNameIsSpecified(RevisionInfo revision)
        {
            return !string.IsNullOrEmpty(revision.Author);
        }

        private static bool AuthorEmailIsSpecified(RevisionInfo revision)
        {
            return !string.IsNullOrEmpty(revision.Email);
        }

        protected override MappingLookup GetTpUserFromMapping(RevisionInfo revision)
        {
            var userMapping = StorageRepository().GetProfile<TfsPluginProfile>().UserMapping;
            MappingLookup lookup = null;

            if (AuthorNameIsSpecified(revision))
            {
                lookup = userMapping[revision.Author];

                if (lookup != null)
                    return lookup;

                var login = GetDomainLogin(revision.Author);

                if (login != null)
                    lookup = userMapping[login];
            }

            return lookup;
        }

        private string GetDomainLogin(string domainLogin)
        {
            var loginSplit = domainLogin.Split('\\');

            return loginSplit.Length == 2 ? loginSplit[1] : null;
        }
    }
}

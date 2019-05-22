// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Plugins.Toolkit.Repositories;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.SourceControl.Workflow.Workflow
{
    public abstract class UserMapper
    {
        protected readonly Func<IStorageRepository> StorageRepository;
        private readonly Func<IActivityLogger> _logger;
        public static readonly MappingLookup DefaultUser = new MappingLookup();

        protected UserMapper(Func<IStorageRepository> storageRepository, Func<IActivityLogger> logger)
        {
            StorageRepository = storageRepository;
            _logger = logger;
        }

        public MappingLookup GetAuthorBy(RevisionInfo revision)
        {
            if (!AuthorIsSpecified(revision))
            {
                _logger().Warn($"No author in revision. Revision ID: {revision.Id}");
                return DefaultUser;
            }

            var tpUser = GetTpUserFromMapping(revision);
            var userDtos = new DataRepository<TpUserData>(StorageRepository()).GetAll().ToList();

            if (tpUser != null)
            {
                if (userDtos.FirstOrDefault(x => x.ID.GetValueOrDefault() == tpUser.Id) != null)
                {
                    return tpUser;
                }

                _logger().Warn($"TP user not found. TargetProcess User ID: {tpUser.Id}; Name: {tpUser.Name}");
            }

            var user = GuessUser(revision, userDtos);

            if (user != null)
            {
                return user.ConvertToUserLookup();
            }

            _logger()
                .Warn($"Revision author doesn't match any TP User name. There is no valid mapping for user {revision.Author}");
            return DefaultUser;
        }

        protected abstract TpUserData GuessUser(RevisionInfo revision, ICollection<TpUserData> userDtos);

        protected abstract bool AuthorIsSpecified(RevisionInfo revision);

        protected abstract MappingLookup GetTpUserFromMapping(RevisionInfo revision);

        public bool IsAuthorMapped(RevisionInfo revision)
        {
            return !GetAuthorBy(revision).Equals(DefaultUser);
        }
    }
}

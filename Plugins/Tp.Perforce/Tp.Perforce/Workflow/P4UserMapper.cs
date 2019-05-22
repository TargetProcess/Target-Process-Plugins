using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.SourceControl;
using Tp.SourceControl.VersionControlSystem;
using Tp.SourceControl.Workflow.Workflow;

namespace Tp.Perforce.Workflow
{
    public class P4UserMapper : UserMapper
    {
        public P4UserMapper(Func<IStorageRepository> storageRepository, Func<IActivityLogger> logger)
            : base(storageRepository, logger)
        {
        }

        protected override TpUserData GuessUser(RevisionInfo revision, ICollection<TpUserData> userDtos)
        {
            TpUserData result = null;

            if (AuthorEmailIsSpecified(revision))
            {
                result = userDtos.FirstOrDefault(x => revision.Email.Equals(x.Email, StringComparison.OrdinalIgnoreCase));
            }

            if (result == null && AuthorNameIsSpecified(revision))
            {
                result =
                    userDtos.FirstOrDefault(x => revision.Author.Equals(string.Join(" ", new[]{x.FirstName, x.LastName}.Where(y => !string.IsNullOrEmpty(y))), StringComparison.OrdinalIgnoreCase))
                    ??
                    userDtos.FirstOrDefault(x => revision.Author.Equals(x.Login, StringComparison.OrdinalIgnoreCase))
                    ??
                    userDtos.FirstOrDefault(x => revision.Author.Equals(x.Email.Split('@')[0], StringComparison.OrdinalIgnoreCase));
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
            var userMapping = StorageRepository().GetProfile<P4PluginProfile>().UserMapping;
            MappingLookup lookup = null;

            if (AuthorEmailIsSpecified(revision))
            {
                lookup = userMapping[revision.Email];
            }

            if (lookup == null && AuthorNameIsSpecified(revision))
            {
                lookup = userMapping[revision.Author];
            }

            return lookup;
        }
    }
}

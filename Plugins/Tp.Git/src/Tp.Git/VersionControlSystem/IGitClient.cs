using System;
using System.Collections.Generic;
using Tp.Core;
using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.Settings;
using Tp.SourceControl.VersionControlSystem;

namespace Tp.Git.VersionControlSystem
{
    public interface IGitClient
    {
        void Fetch();
        IEnumerable<RevisionRange> GetFromTillHead(DateTime from, int pageSize);
        IEnumerable<RevisionRange> GetAfterTillHead(RevisionId revisionId, int pageSize);
        string[] RetrieveAuthors(DateRange dateRange);
        RevisionInfo[] GetRevisions(RevisionId fromChangeset, RevisionId toChangeset);
        RevisionId GetParent(RevisionId revision);
        IEnumerable<RevisionRange> GetFromAndBefore(RevisionId @from, RevisionId to, int pageSize);
        string GetFileContent(RevisionId revision, string path);
    }

    public interface IGitClientFactory
    {
        IGitClient Create(ISourceControlConnectionSettingsSource settings, IStorage<GitRepositoryFolder> folderStorage);
    }
}

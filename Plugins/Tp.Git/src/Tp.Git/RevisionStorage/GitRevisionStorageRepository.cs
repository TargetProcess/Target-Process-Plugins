using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.RevisionStorage;

namespace Tp.Git.RevisionStorage
{
    public class GitRevisionStorageRepository : RevisionStorageRepository
    {
        public GitRevisionStorageRepository(IStorageRepository repository, IProfileCollectionReadonly profiles) : base(repository, profiles) { }
    }
}

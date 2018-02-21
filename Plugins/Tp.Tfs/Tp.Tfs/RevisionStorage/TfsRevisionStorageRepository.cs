using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.RevisionStorage;

namespace Tp.Tfs.RevisionStorage
{
    public class TfsRevisionStorageRepository : RevisionStorageRepository
    {
        public TfsRevisionStorageRepository(IStorageRepository repository, IProfileCollectionReadonly profiles) : base(repository, profiles) { }
    }
}

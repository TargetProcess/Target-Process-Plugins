using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.RevisionStorage;

namespace Tp.Mercurial.RevisionStorage
{
    public class MercurialRevisionStorageRepository : RevisionStorageRepository
    {
        public MercurialRevisionStorageRepository(IStorageRepository repository, IProfileCollectionReadonly profiles) : base(repository, profiles) { }
    }
}

using Tp.Integration.Plugin.Common.Domain;
using Tp.SourceControl.RevisionStorage;

namespace Tp.Perforce.RevisionStorage
{
   public class P4RevisionStorageRepository : RevisionStorageRepository
    {
        public P4RevisionStorageRepository(IStorageRepository repository, IProfileCollectionReadonly profiles) : base(repository, profiles) { }
    }
}

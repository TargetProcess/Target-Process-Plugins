using System;
using Tp.Integration.Messages;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;

namespace Tp.Search.Tests
{
    class SearchProfile : IProfileReadonly
    {
        public SearchProfile()
        {
            Name = "SearchProfile";
        }

        public bool IsNull
        {
            get { return false; }
        }

        public IStorage<T> Get<T>()
        {
            throw new NotImplementedException();
        }

        public IStorage<T> Get<T>(params StorageName[] storageNames)
        {
            throw new NotImplementedException();
        }

        public T GetProfile<T>()
        {
            throw new NotImplementedException();
        }

        public ProfileName Name { get; private set; }
        public object Settings { get; private set; }
        public bool Initialized { get; set; }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StructureMap;
using Tp.Core;
using Tp.Core.Annotations;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.SourceControl;

namespace Tp.Git.Tests
{
    [TestFixture]
    public class NewProfileInitializationSagaTests
    {        
        [Test]
        public void ShouldNotAddInactiveAndDeletedUsers()
        {
            var storageRepository = new StorageRepositoryMock();
            ObjectFactory.Configure(c => c.For<IStorageRepository>().Use(storageRepository));

            var saga = new NewVcsProfileInitializationSaga { Data = new NewProfileInitializationSagaData { } };
            saga.Handle(new UserQueryResult
            {
                QueryResultCount = 42,
                Dtos = new[]
                {
                    new UserDTO { ID = 1, DeleteDate = CurrentDate.Value, IsActive = true },
                    new UserDTO { ID = 2, DeleteDate = CurrentDate.Value, IsActive = false },
                    new UserDTO { ID = 3, DeleteDate = null, IsActive = false },
                    new UserDTO { ID = 4, DeleteDate = null, IsActive = true },
                }
            });

            Assert.AreEqual(1, storageRepository.Storages.Count);
            Assert.AreEqual(4, ((IStorage<TpUserData>)storageRepository.Storages["4"]).Single().ID);
        }

        class StorageMock<T> : IStorage<T>
        {
            private readonly List<T> _list = new List<T>();

            public bool IsNull => false;

            public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void ReplaceWith(params T[] value)
            {
                throw new NotImplementedException();
            }

            public void Update(T value, Predicate<T> condition)
            {
                throw new NotImplementedException();
            }

            public void AddRange(IEnumerable<T> items)
            {
                throw new NotImplementedException();
            }

            public void Remove(Predicate<T> condition)
            {
                throw new NotImplementedException();
            }

            public void Add(T item) => _list.Add(item);

            public void Clear()
            {
                throw new NotImplementedException();
            }
        }

        [UsedImplicitly]
        class StorageRepositoryMock : IStorageRepository
        {
            private readonly Dictionary<string, object> _storages = new Dictionary<string, object>();

            public IReadOnlyDictionary<string, object> Storages => _storages;

            public bool IsNull => false;

            public IStorage<T> Get<T>()
            {
                throw new NotImplementedException();
            }

            public IStorage<T> Get<T>(params StorageName[] storageNames)
            {
                return (IStorage<T>)_storages.GetOrAdd(storageNames.Single().Value, _ => new StorageMock<T>());
            }

            public T GetProfile<T>()
            {
                throw new NotImplementedException();
            }
        }
    }
}
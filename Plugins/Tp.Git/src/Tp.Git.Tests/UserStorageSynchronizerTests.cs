using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Plugins.Toolkit.Repositories;
using Tp.SourceControl;

namespace Tp.Git.Tests
{
    [TestFixture]
    public class UserStorageSynchronizerTests 
    {
        private UserRepositoryMock _repository;
        private UserStorageSynchronizer _synchronizer;

        class UserRepositoryMock : IRepository<TpUserData>
        {
            private readonly List<TpUserData> _list = new List<TpUserData>();

            public IEnumerable<TpUserData> GetAll() => _list;

            public void Add(TpUserData entity) => _list.Add(entity);

            public void Delete(TpUserData entity) => _list.RemoveAll(x => x.ID == entity.ID);

            public void Update(TpUserData entity)
            {
                Delete(entity);
                Add(entity);
            }

            public TpUserData Find(int? id) => _list.FirstOrDefault(x => x.ID == id);
        }

        [SetUp]
        public void Setup()
        {
            _repository = new UserRepositoryMock();
            _synchronizer = new UserStorageSynchronizer(_repository);
        }

        [Test]
        public void ShouldNotAddDeletedUser()
        {
            _synchronizer.Handle(new UserCreatedMessage { Dto = new UserDTO { DeleteDate = CurrentDate.Value } });
            Assert.IsEmpty(_repository.GetAll());
        }

        [Test]
        public void ShouldNotAddInactiveUser()
        {
            _synchronizer.Handle(new UserCreatedMessage { Dto = new UserDTO { IsActive = false } });
            Assert.IsEmpty(_repository.GetAll());
        }

        [Test]
        public void ShouldRemoveUserWhenItDeleted()
        {
            var user = new TpUserData { ID = 1 };
            _repository.Add(user);
            _synchronizer.Handle(new UserUpdatedMessage
            {
                Dto = new UserDTO { ID = 1, DeleteDate = CurrentDate.Value },
                ChangedFields = new[] { UserField.DeleteDate }
            });
            Assert.IsEmpty(_repository.GetAll());
        }

        [Test]
        public void ShouldRemoveUserWhenItMadeInactive()
        {
            var user = new TpUserData { ID = 1 };
            _repository.Add(user);
            _synchronizer.Handle(new UserUpdatedMessage
            {
                Dto = new UserDTO { ID = 1, IsActive = false },
                ChangedFields = new[] { UserField.IsActive }
            });
            Assert.IsEmpty(_repository.GetAll());
        }

        [Test]
        public void ShouldAddUserWhenItMadeActive()
        {
            _synchronizer.Handle(new UserUpdatedMessage
            {
                Dto = new UserDTO { ID = 1, IsActive = true, DeleteDate = null },
                ChangedFields = new[] { UserField.IsActive }
            });
            Assert.IsNotEmpty(_repository.GetAll());
        }

        [Test]
        public void ShouldAddUserWhenItUndelete()
        {
            _synchronizer.Handle(new UserUpdatedMessage
            {
                Dto = new UserDTO { ID = 1, IsActive = true, DeleteDate = null },
                ChangedFields = new[] { UserField.DeleteDate }
            });
            Assert.IsNotEmpty(_repository.GetAll());
        }

        [Test]
        public void ShouldAddUserWhenItUndeleteAndMadeActive()
        {
            _synchronizer.Handle(new UserUpdatedMessage
            {
                Dto = new UserDTO { ID = 1, IsActive = true, DeleteDate = null },
                ChangedFields = new[] { UserField.DeleteDate, UserField.IsActive }
            });
            Assert.IsNotEmpty(_repository.GetAll());
        }

        [Test]
        public void ShouldNotAddUserWhenItMadeActiveButStillDeleted()
        {
            _synchronizer.Handle(new UserUpdatedMessage
            {
                Dto = new UserDTO { ID = 1, IsActive = true, DeleteDate = CurrentDate.Value },
                ChangedFields = new[] { UserField.IsActive }
            });
            Assert.IsEmpty(_repository.GetAll());
        }

        [Test]
        public void ShouldAddUserWhenItUndeleteButStillInactive()
        {
            _synchronizer.Handle(new UserUpdatedMessage
            {
                Dto = new UserDTO { ID = 1, IsActive = false, DeleteDate = null },
                ChangedFields = new[] { UserField.DeleteDate }
            });
            Assert.IsEmpty(_repository.GetAll());
        }

        [Test]
        public void ShouldNotUpdateIfChangedIrrelevantField()
        {
            var user = new TpUserData { ID = 1 };
            _repository.Add(user);
            _synchronizer.Handle(new UserUpdatedMessage
            {
                Dto = new UserDTO { ID = 1, IsActive = true, DeleteDate = null, CustomField1 = "foo" },
                ChangedFields = new[] { UserField.CustomField1 }
            });
            Assert.IsTrue(ReferenceEquals(user, _repository.GetAll().Single()));
        }

        [Test]
        public void ShouldUpdateIfChangedRelevantField()
        {
            var user = new TpUserData { ID = 1, FirstName = "John" };
            _repository.Add(user);
            _synchronizer.Handle(new UserUpdatedMessage
            {
                Dto = new UserDTO { ID = 1, IsActive = true, DeleteDate = null, FirstName = "Johnny" },
                ChangedFields = new[] { UserField.FirstName }
            });
            Assert.AreEqual("Johnny", _repository.GetAll().Single().FirstName);
        }
    }
}

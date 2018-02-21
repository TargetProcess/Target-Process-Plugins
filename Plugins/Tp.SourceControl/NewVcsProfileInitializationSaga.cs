using System.Linq;
using NServiceBus;
using StructureMap;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Plugins.Toolkit.Repositories;

namespace Tp.SourceControl
{
    public class NewVcsProfileInitializationSaga
        : NewProfileInitializationSaga<NewProfileInitializationSagaData>,
          IHandleMessages<UserQueryResult>
    {
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<UserQueryResult>(saga => saga.Id, message => message.SagaId);
        }

        protected override void OnStartInitialization()
        {
            ObjectFactory.GetInstance<IActivityLogger>().Info("Initializing profile");

            Data.AllUsersCount = int.MinValue;
            Send(new RetrieveAllUsersQuery());
        }

        public void Handle(UserQueryResult message)
        {
            var userRepository = new DataRepository<TpUserData>(StorageRepository());
            Data.AllUsersCount = message.QueryResultCount;
            if (message.Dtos != null)
            {
                Data.UsersRetrievedCount += message.Dtos.Length;
                message.Dtos
                    .Where(u => u.DeleteDate == null && u.IsActive == true)
                    .ForEach(u => userRepository.Add(new TpUserData(u)));
            }
            CompleteSagaIfNecessary();
        }

        private void CompleteSagaIfNecessary()
        {
            if (Data.AllUsersCount == Data.UsersRetrievedCount)
            {
                MarkAsComplete();
            }
        }
    }
}

using NServiceBus;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.SourceControl
{
	public class NewVcsProfileInitializationSaga : NewProfileInitializationSaga<NewProfileInitializationSagaData>,
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
			Data.AllUsersCount = message.QueryResultCount;
			if (message.Dtos != null)
			{
				Data.UsersRetrievedCount += message.Dtos.Length;
				foreach (var userDto in message.Dtos)
				{
					StorageRepository().Get<UserDTO>().Add(userDto);
				}
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
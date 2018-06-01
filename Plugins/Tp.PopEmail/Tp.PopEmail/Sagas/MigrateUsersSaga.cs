// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NServiceBus;
using NServiceBus.Saga;
using StructureMap;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.TargetProcessLifecycle;
using Tp.Integration.Plugin.Common;
using Tp.PopEmailIntegration.Data;
using Tp.PopEmailIntegration.Initialization;
using IProfile = Tp.Integration.Plugin.Common.Domain.IProfile;

namespace Tp.PopEmailIntegration.Sagas
{
    public class MigrateUsersSaga
        : TpSaga<MigrateUsersSagaSagaData>, IAmStartedByMessages<MigrateUsersCommandInternal>,
          IHandleMessages<UserQueryResult>,
          IHandleMessages<RequesterQueryResult>,
          IHandleMessages<TargetProcessExceptionThrownMessage>
    {
        private Func<IProfile> _profile;

        public MigrateUsersSaga()
        {
            _profile = () =>
            {
                var profile = ObjectFactory.GetInstance<IProfile>();
                _profile = () => profile;
                return profile;
            };
        }

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<UserQueryResult>(saga => saga.Id, message => message.SagaId);
            ConfigureMapping<RequesterQueryResult>(saga => saga.Id, message => message.SagaId);
            ConfigureMapping<TargetProcessExceptionThrownMessage>(saga => saga.Id, message => message.SagaId);
        }

        public void Handle(MigrateUsersCommandInternal message)
        {
            if (_profile().Initialized)
            {
                MarkAsComplete();
                return;
            }
            ObjectFactory.GetInstance<UserRepository>().RemoveAll();

            Data.AllUsersCount = int.MinValue;
            Data.AllRequestersCount = int.MinValue;

            Send(new RetrieveAllUsersQuery());
            Send(new RetrieveAllRequestersQuery());
            Log().Info("Started Users migration");
        }

        public void Handle(UserQueryResult message)
        {
            Data.AllUsersCount = message.QueryResultCount;
            if (message.Dtos != null)
            {
                Data.UsersRetrievedCount += message.Dtos.Length;
                foreach (var userDto in message.Dtos)
                {
                    AddUserLite(UserLite.Create(userDto));
                }
            }
            CompleteSagaIfNecessary();
        }

        private void CompleteSagaIfNecessary()
        {
            if (Data.UsersRetrievedCount == Data.AllUsersCount && Data.RequestersRetrievedCount == Data.AllRequestersCount)
            {
                MarkAsComplete();
                var profileSettings = (ProjectEmailProfile)_profile().Settings;
                profileSettings.UsersMigrated = true;
                _profile().ToggleMessageHandling(false);
                Log().Info("Finished Users migration");
            }
        }

        private static void AddUserLite(UserLite userLite)
        {
            ObjectFactory.GetInstance<UserRepository>().Add(userLite);
        }

        public void Handle(RequesterQueryResult message)
        {
            Data.AllRequestersCount = message.QueryResultCount;
            if (message.Dtos != null)
            {
                foreach (var userDto in message.Dtos)
                {
                    AddUserLite(UserLite.Create(userDto));
                }

                Data.RequestersRetrievedCount += message.Dtos.Length;
            }
            CompleteSagaIfNecessary();
        }

        public void Handle(TargetProcessExceptionThrownMessage message)
        {
            Log().Error("Failed to Migrate Users:", message.GetException());
            _profile().ToggleMessageHandling(false);
            MarkAsComplete();
        }
    }

    public class MigrateUsersSagaSagaData : ISagaEntity
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        public int UsersRetrievedCount { get; set; }
        public int RequestersRetrievedCount { get; set; }

        public int AllUsersCount { get; set; }
        public int AllRequestersCount { get; set; }
    }
}

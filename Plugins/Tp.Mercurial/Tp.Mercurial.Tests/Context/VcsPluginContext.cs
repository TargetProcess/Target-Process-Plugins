// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Testing.Common;
using Tp.Mercurial;
using Tp.SourceControl.Commands;
using Tp.SourceControl.VersionControlSystem;
using Tp.Subversion.StructureMap;

namespace Tp.Mercurial.Tests.Context
{
	public class VcsPluginContext
	{
		private readonly TransportMock _transport;
		private IProfileReadonly _profileStorage;
		private readonly ContextExpectations _contextExpectations;
		public List<TpUserMappingInfo> TpUsers { get; private set; }
		private MercurialPluginProfile _profile;

		public VcsPluginContext(TransportMock transportMock)
		{
			_transport = transportMock;

			_contextExpectations = new ContextExpectations(this);
			TpUsers = new List<TpUserMappingInfo>();

			Transport.On<RetrieveAllUsersQuery>()
				.Reply(x => CreateRetrieveAllUsersQueryReply());

			On.CreateCommand<RevisionDTO>().Reply(CreateRevisionCreatedMessage);
			On.CreateCommand<RevisionFileDTO>().Reply(CreateRevisionFileCreatedMessage);
			On.CreateCommand<RevisionAssignableDTO>().Reply(CreateRevisionAssignableCreatedMessage);
		}

        public MercurialPluginProfile Profile
		{
            get { return _profile ?? (_profile = ProfileStorage.GetProfile<MercurialPluginProfile>()); }
		}

		public TransportMock Transport
		{
			get { return _transport; }
		}

		public LogMock Log
		{
			get { return ObjectFactory.GetInstance<LogMock>(); }
		}

		private ISagaMessage[] CreateRetrieveAllUsersQueryReply()
		{
			if (!UserDtos.Any())
			{
				return new[] {new UserQueryResult {Dtos = null, QueryResultCount = 0}};
			}

			return UserDtos.Select(
				dto => new UserQueryResult {Dtos = new[] {dto}, QueryResultCount = UserDtos.Count()}).ToArray();
		}

		public IEnumerable<UserDTO> UserDtos
		{
			get
			{
				return TpUsers.Select(u =>
				                      	{
				                      		var firstName = u.Name;
				                      		var lastName = string.Empty;

				                      		var index = u.Name.IndexOf(' ');

											if (index > -1)
											{
												firstName = u.Name.Substring(0, index);
												lastName = u.Name.Substring(index + 1);
											}

				                      		return new UserDTO
				                      			{
				                      				ID = u.Id,
				                      				Login = u.Login,
				                      				FirstName = firstName,
				                      				LastName = lastName,
				                      				IsActive = true,
				                      				Email = u.Email
				                      			};
				                      	}).ToArray();
			}
		}

		#region Configuration

		public ContextExpectations On
		{
			get { return _contextExpectations; }
		}

		#endregion

		private static ISagaMessage CreateRevisionAssignableCreatedMessage(CreateCommand createCommand)
		{
			return new RevisionAssignableCreatedMessage {Dto = (RevisionAssignableDTO) createCommand.Dto};
		}

		public List<RevisionInfo> Revisions
		{
			get { return ((VersionControlSystemMock) VersionControlSystem).Revisions; }
		}

		private IProfileReadonly ProfileStorage
		{
			get { return _profileStorage ?? (_profileStorage = _transport.AddProfile("Test profile", new MercurialPluginProfile { StartRevision = "1/1/1980", Uri = "http://localhost" })); }
		}

		private IVersionControlSystem VersionControlSystem
		{
			get { return ObjectFactory.GetInstance<IVersionControlSystem>(); }
		}

		private static ISagaMessage CreateRevisionFileCreatedMessage(CreateCommand createCommand)
		{
			var revisionFile = (RevisionFileDTO) createCommand.Dto;
			revisionFile.ID = EntityId.Next();
			return new RevisionFileCreatedMessage { Dto = revisionFile };
		}

		private static ISagaMessage CreateRevisionCreatedMessage(CreateCommand command)
		{
			var revision = (RevisionDTO) command.Dto;
			revision.ID = EntityId.Next();
			return new RevisionCreatedMessage {Dto = revision};
		}

		public void StartPlugin()
		{
			_transport.LocalQueue.Clear();
			_transport.TpQueue.Clear();
			_transport.HandleLocalMessage(ProfileStorage, new TickMessage());
		}

		public void CreateTpUser(string name, int id)
		{
			TpUsers.Add(new TpUserMappingInfo
			            	{Name = name, Id = id, Email = Guid.NewGuid() + "@mail.com", Login = Guid.NewGuid() + "_login"});
		}

		public void MapUser(string vcsuser, string tpuserName)
		{
			Profile.UserMapping.Add(new MappingElement
			                        	{
			                        		Key = vcsuser,
			                        		Value =
			                        			new MappingLookup {Name = tpuserName, Id = TpUsers.Single(x => x.Name == tpuserName).Id}
			                        	});
		}

		public TpUserMappingInfo GetTpUserByName(string userName)
		{
			return TpUsers.FirstOrDefault(x => x.Name == userName);
		}

		public TpUserMappingInfo GetTpUserByLogin(string userLogin)
		{
			return TpUsers.FirstOrDefault(x => x.Login == userLogin);
		}


		public void CreateTpUser(string name, string mail)
		{
			var userLookup = new TpUserMappingInfo
			                 	{Id = EntityId.Next(), Name = name, Email = mail, Login = Guid.NewGuid() + "_login"};
			TpUsers.Add(userLookup);
		}

		public void CreateTpUser(string name)
		{
			CreateTpUser(name, name + "@company.com");
		}

		public void CreateTpUser(string userName, string userLogin, int userId)
		{
			CreateTpUser(userName, userLogin, Guid.NewGuid() + "@mail.com", userId);
		}

		public void CreateTpUser(string userName, string userLogin, string email, int userId)
		{
			var userLookup = new TpUserMappingInfo { Id = EntityId.Next(), Name = userName, Login = userLogin, Email = email };
			TpUsers.Add(userLookup);
		}

		public void CreateTpUserWithLogin(string login, string name, string mail)
		{
			var userLookup = new TpUserMappingInfo { Id = EntityId.Next(), Name = name, Login = login, Email = mail };
			TpUsers.Add(userLookup);
		}
	}
}

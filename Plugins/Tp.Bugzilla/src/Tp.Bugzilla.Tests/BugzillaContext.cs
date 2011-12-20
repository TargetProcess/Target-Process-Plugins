// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rhino.Mocks;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using Tp.Bugzilla.BugFieldConverters;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Bugzilla.CustomCommand.Dtos;
using Tp.Bugzilla.ImportToTp;
using Tp.Bugzilla.Tests.Mocks;
using Tp.Core;
using Tp.Integration.Common;
using Tp.Integration.Messages.Commands;
using Tp.Integration.Messages.EntityLifecycle;
using Tp.Integration.Messages.EntityLifecycle.Messages;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Integration.Testing.Common;
using Tp.Plugin.Core;

namespace Tp.Bugzilla.Tests
{
	public class BugzillaContextRegistry : Registry
	{
		public BugzillaContextRegistry()
		{
			For<BugzillaContext>().HybridHttpOrThreadLocalScoped().Use<BugzillaContext>();
		}
	}

	public class BugzillaContext
	{
		public List<EntityStateDTO> EntityStates { get; private set; }
		public List<PriorityDTO> Priorities { get; private set; }
		public List<SeverityDTO> Severities { get; private set; }
		public List<ProjectDTO> Projects { get; private set; }
		public List<BugDTO> TpBugs { get; private set; }
		public List<UserDTO> Users { get; private set; }
		public List<RoleDTO> Roles { get; set; }
		public List<CommentDTO> TpComments { get; private set; }
		public List<AttachmentDTO> TpAttachments { get; private set; }
		public List<TeamDTO> TpTeams { get; private set; }
		public List<BugzillaBugInfo> BugzillaBugInfos { get; private set; }
		public MappingSource MappingSource { get; private set; }
		private readonly List<int> _ids = new List<int>();
		public PluginCommandResponseMessage CommandResponse { get; set; }

		public BugzillaContext()
		{
			Users = new List<UserDTO>();
			EntityStates = new List<EntityStateDTO>();
			Priorities = new List<PriorityDTO>();
			Severities = new List<SeverityDTO>();
			Projects = new List<ProjectDTO>();
			TpBugs = new List<BugDTO>();
			Roles = new List<RoleDTO>();
			TpComments = new List<CommentDTO>();
			TpTeams = new List<TeamDTO>();
			TpAttachments = new List<AttachmentDTO>();
			BugzillaBugInfos = new List<BugzillaBugInfo>();
			MappingSource = new MappingSource
			                	{
			                		States = new MappingSourceEntry
			                		         	{
			                		         		ThirdPartyItems = new List<string>(),
			                		         		TpItems = new List<MappingLookup>()
			                		         	},
			                		Severities = new MappingSourceEntry
			                		             	{
			                		             		ThirdPartyItems = new List<string>(),
			                		             		TpItems = new List<MappingLookup>()
			                		             	},
			                		Priorities = new MappingSourceEntry
			                		             	{
			                		             		ThirdPartyItems = new List<string>(),
			                		             		TpItems = new List<MappingLookup>()
			                		             	},
			                		Roles = new MappingSourceEntry
			                		        	{
			                		        		ThirdPartyItems = new List<string>(),
			                		        		TpItems = new List<MappingLookup>()
			                		        	}
			                	};
		}

		public IBugzillaInfoStorageRepository StorageRepository
		{
			get
			{
				return
					ObjectFactory.GetInstance<IBugzillaInfoStorageRepository>(
						new ExplicitArguments(new Dictionary<string, object>
						                      	{{"repository", ObjectFactory.GetInstance<BugzillaServiceMock>().CurrentProfile}}));
			}
		}

		public void Initialize()
		{
			ObjectFactory.Configure(
				x => x.For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof (BugzillaProfile).Assembly,
				                                                                             new List<Assembly>
				                                                                             	{
				                                                                             		typeof (ExceptionThrownLocalMessage).
				                                                                             			Assembly,
				                                                                             		typeof (BugzillaProfile).Assembly
				                                                                             	},
				                                                                             new Assembly[] {})));
			ObjectFactory.Configure(x =>
			                        	{
			                        		x.For<IBugzillaService>().HybridHttpOrThreadLocalScoped().Use<BugzillaServiceMock>();
			                        		x.Forward<IBugzillaService, BugzillaServiceMock>();
			                        		x.For<IBugConverter>().HybridHttpOrThreadLocalScoped().Use<ConverterComposite>();
			                        		var bugChunkSize = MockRepository.GenerateStub<IBugChunkSize>();
			                        		bugChunkSize.Stub(y => y.Value).Return(1);
			                        		x.For<IBugChunkSize>().HybridHttpOrThreadLocalScoped().Use(bugChunkSize);
			                        		x.For<IBugzillaInfoStorageRepository>().HybridHttpOrThreadLocalScoped().Use
			                        			<BugzillaInfoStorageRepository>();
			                        		x.For<IBugzillaActionFactory>().HybridHttpOrThreadLocalScoped().Use<BugzillaActionFactory>();
			                        	});

			AddReplyForCreateCommand<BugDTO, BugCreatedMessage>(TpBugs);
			AddReplyForUpdateCommand<BugDTO, BugField, BugUpdatedMessage>(TpBugs, BugField.EntityStateID);
			AddReplyForCreateCommand<CommentDTO, CommentCreatedMessage>(TpComments);
			AddReplyForUpdateCommand<CommentDTO, CommentField, CommentUpdatedMessage>(TpComments);
			AddReplyForCreateCommand<AttachmentDTO, AttachmentCreatedMessage>(TpAttachments);

			AddReplyForCreateCommand<TeamDTO, TeamCreatedMessage>(TpTeams);
			TransportMock.OnDeleteEntityCommand<TeamDTO>().Reply(
				x =>
					{
						var dto =
							ObjectFactory.GetInstance<IStorageRepository>().Get<TeamDTO>().SingleOrDefault(teamDto => teamDto.ID == x.ID);

						var teamDeletedMessage = new TeamDeletedMessage {Dto = dto ?? new TeamDTO {ID = x.ID}};
						return teamDeletedMessage;
					});

			AddReplyForQuery<RetrieveAllUsersQuery, UserDTO, UserQueryResult>(Users);
			AddReplyForQuery<EntityStateQuery, EntityStateDTO, EntityStateQueryResult>(EntityStates);
			AddReplyForQuery<PriorityQuery, PriorityDTO, PriorityQueryResult>(Priorities);
			AddReplyForQuery<RetrieveAllSeveritiesQuery, SeverityDTO, SeverityQueryResult>(Severities);
			AddReplyForQuery<RetrieveAllRolesQuery, RoleDTO, RoleQueryResult>(Roles);
			AddReplyForQuery<RetrieveAllProjectsQuery, ProjectDTO, ProjectQueryResult>(Projects);
		}

		private void AddReplyForCreateCommand<TDto, TReplyMessage>(ICollection<TDto> dtos)
			where TReplyMessage : EntityCreatedMessage<TDto>, ISagaMessage, new()
			where TDto : DataTransferObject, new()
		{
			TransportMock.OnCreateEntityCommand<TDto>().Reply(
				x =>
					{
						x.ID = GetNextId();
						dtos.Add(x);
						return new TReplyMessage {Dto = x};
					});
		}

		private void AddReplyForUpdateCommand<TDto, TEntityField, TReplyMessage>(ICollection<TDto> dtos,
		                                                                         params TEntityField[] changedFields)
			where TReplyMessage : EntityUpdatedMessage<TDto, TEntityField>, ISagaMessage, new()
			where TEntityField : IConvertible
			where TDto : DataTransferObject, new()
		{
			if (changedFields.Length == 0)
			{
				return;
			}

			TransportMock.OnUpdateEntityCommand<TDto>().Reply(
				x =>
					{
						dtos.Remove(dtos.Single(storedBug => storedBug.ID == x.ID));
						dtos.Add(x);
						return new TReplyMessage {Dto = x, ChangedFields = changedFields};
					});
		}

		private static void AddReplyForQuery<TQuery, TDto, TResult>(List<TDto> dtos)
			where TQuery : QueryBase
			where TDto : DataTransferObject
			where TResult : QueryResult<TDto>, ISagaMessage, new()
		{
			ObjectFactory.GetInstance<TransportMock>().On<TQuery>()
				.Reply(x =>
				       	{
				       		if (!dtos.Any())
				       		{
				       			return new[] {new TResult {Dtos = null, QueryResultCount = 0}};
				       		}
				       		return dtos.Select(
				       			dto => new TResult {Dtos = new[] {dto}, QueryResultCount = dtos.Count()}).ToArray();
				       	});
		}

		public int GetNextId()
		{
			var result = !_ids.Any() ? 1 : _ids.Max() + 1;
			_ids.Add(result);
			return result;
		}

		public BugzillaBugCollection BugzillaBugs
		{
			get { return ObjectFactory.GetInstance<BugzillaServiceMock>().Bugs; }
		}

		public void SetProfile(IProfileReadonly profile)
		{
			ObjectFactory.GetInstance<BugzillaServiceMock>().InnerProfile = profile;
		}

		public void AddProfileWithDefaultRolesMapping(int projectId, BugzillaProfile profile)
		{
			AddProfileWithDefaultRolesMapping("TestProfile", projectId, profile);
		}

		public void AddProfileWithDefaultRolesMapping(int projectId)
		{
			AddProfileWithDefaultRolesMapping("TestProfile", projectId);
		}

		public void AddProfileWithDefaultRolesMapping(string profileName, int projectId)
		{
			TransportMock.AddProfile(profileName, GetBugzillaProfileWithDefaultRolesMapping(projectId));
		}

		public void AddProfileWithDefaultRolesMapping(string profileName, int projectId, BugzillaProfile profile)
		{
			SetDefaultRolesMapping(profile);
			TransportMock.AddProfile(profileName, profile);
		}

		public void AddProfile(int projectId)
		{
			AddProfile("TestProfile", projectId);
		}

		public void AddProfile(string profileName, int projectId)
		{
			TransportMock.AddProfile(profileName, GetBugzillaProfile(projectId));
		}

		public static BugzillaProfile GetBugzillaProfile(int projectId)
		{
			return new BugzillaProfile
			       	{
			       		Project = projectId,
			       		Login = "login",
			       		Password = "password",
			       		Queries = "query123",
			       		Url = "http://test/com"
			       	};
		}

		public void CreateDefaultRolesIfNecessary()
		{
			if (!Roles.Any(x => x.Name == "Developer"))
			{
				CreateRole("Developer");
			}
			if (!Roles.Any(x => x.Name == "QA Engineer"))
			{
				CreateRole("QA Engineer");
			}
		}

		public void CreateRole(string roleName)
		{
			Roles.Add(new RoleDTO {Name = roleName, ID = GetNextId()});
		}

		private void SetDefaultRolesMapping(BugzillaProfile profile)
		{
			var roles = Roles;
			var assignee = roles.Where(x => x.Name == "Developer").Single();
			var reporter = roles.Where(x => x.Name == "QA Engineer").Single();

			profile.RolesMapping = new MappingContainer
			                       	{
			                       		new MappingElement
			                       			{
			                       				Key = DefaultRoles.Assignee,
			                       				Value = new MappingLookup {Id = assignee.ID.Value, Name = assignee.Name}
			                       			},
			                       		new MappingElement
			                       			{
			                       				Key = DefaultRoles.Reporter,
			                       				Value = new MappingLookup {Id = reporter.ID.Value, Name = reporter.Name}
			                       			}
			                       	};
		}

		private object GetBugzillaProfileWithDefaultRolesMapping(int projectId)
		{
			var profile = GetBugzillaProfile(projectId);

			SetDefaultRolesMapping(profile);

			return profile;
		}

		private static TransportMock TransportMock
		{
			get { return ObjectFactory.GetInstance<TransportMock>(); }
		}

		public TimeSpan TimeOffset
		{
			get
			{
				var now = CurrentDate.Value;
				return now - now.ToUniversalTime();
			}
		}
	}
}
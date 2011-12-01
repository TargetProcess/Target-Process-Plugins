// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Microsoft.Web.Services3;
using StructureMap;
using StructureMap.Configuration.DSL;
using Tp.AssignableServiceProxy;
using Tp.AttachmentServiceProxy;
using Tp.AuditHistoryServiceProxy;
using Tp.AuthenticationServiceProxy;
using Tp.BugServiceProxy;
using Tp.BuildServiceProxy;
using Tp.CommentServiceProxy;
using Tp.CustomActivityServiceProxy;
using Tp.CustomFieldServiceProxy;
using Tp.EntityStateServiceProxy;
using Tp.FeatureServiceProxy;
using Tp.FileServiceProxy;
using Tp.GeneralServiceProxy;
using Tp.GeneralUserServiceProxy;
using Tp.ImpedimentServiceProxy;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Events.Aggregator;
using Tp.Integration.Plugin.Common.Gateways;
using Tp.Integration.Plugin.Common.Logging;
using Tp.Integration.Plugin.Common.PluginCommand;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.Integration.Plugin.Common.Storage.Repositories;
using Tp.IterationServiceProxy;
using Tp.MyAssignmentsServiceProxy;
using Tp.PasswordRecoveryServiceProxy;
using Tp.PriorityServiceProxy;
using Tp.ProcessServiceProxy;
using Tp.ProgramServiceProxy;
using Tp.ProjectMemberServiceProxy;
using Tp.ProjectServiceProxy;
using Tp.ReleaseServiceProxy;
using Tp.RequestServiceProxy;
using Tp.RequestTypeServiceProxy;
using Tp.RequesterServiceProxy;
using Tp.RevisionFileServiceProxy;
using Tp.RevisionServiceProxy;
using Tp.RoleEffortServiceProxy;
using Tp.RoleServiceProxy;
using Tp.SeverityServiceProxy;
using Tp.SolutionServiceProxy;
using Tp.TagsServiceProxy;
using Tp.TaskServiceProxy;
using Tp.TestCaseRunServiceProxy;
using Tp.TestCaseServiceProxy;
using Tp.TestPlanRunServiceProxy;
using Tp.TestPlanServiceProxy;
using Tp.TimeServiceProxy;
using Tp.UserServiceProxy;
using Tp.UserStoryServiceProxy;

namespace Tp.Integration.Plugin.Common.StructureMap
{
	public class PluginRegistry : Registry
	{
		public PluginRegistry()
		{
			For<IPluginSettings>().Singleton().Use<PluginSettings>();
			For<ServiceManager>().Singleton().Use<ServiceManager>();
			ConfigureWebServices();

			For<IPluginContext>().Singleton().Use<PluginContext>();
			For<IProfileGatewayFactory>().Singleton().Use<ProfileGatewayFactory>();

			For<ILogManager>().Singleton().Use<TpLogManager>();

			For<IPluginCurrentObjectContext>().Singleton().Use<PluginCurrentObjectContext>();
			For<IAccountCollection>().Singleton().Use<AccountCollection>();
			For<IProfileCollectionReadonly>().Use<CurrentProfileCollection>();
			For<IProfileCollection>().Use<CurrentProfileCollection>();

			For<IProfile>().Use<CurrentProfile>();
			For<IProfileReadonly>().Use<CurrentProfile>();
			Forward<IProfileReadonly, IStorageRepository>();

			For<IAccountRepository>().Singleton().Use<AccountRepository>();
			For<IProfileRepository>().Singleton().Use<ProfileRepository>();
			Forward<IProfileRepository, IProfileFactory>();

			For<IPluginPersister>().Singleton().Use(GetPluginPersisterInstance);
			For<IAccountPersister>().Singleton().Use(GetAccountPersisterInstance);
			For<IProfilePersister>().Singleton().Use(GetProfilePersisterInstance);
			For<IProfileStoragePersister>().Singleton().Use(GetProfileStoragePersisterInstance);
			For<ITpBus>().Singleton().Use<TpBus>();
			Forward<ITpBus, ICommandBus>();
			Forward<ITpBus, ILocalBus>();
			FillAllPropertiesOfType<ITpBus>();
			FillAllPropertiesOfType<ICommandBus>();

			For<IAssembliesHost>().Singleton().Use(GetAssembliesHost);
			For<IPluginMetadata>().Singleton().Use<AssemblyScanner>();
			For<IDatabaseConfiguration>().Singleton().Use<DatabaseConfiguration>();

			For<IPluginCommandRepository>().Singleton().Use<PluginCommandRepository>();
			Forward<IPluginCommandRepository, PluginCommandRepository>();

			For<PluginRuntime>().Singleton().Use<PluginRuntime>();
			For<IEventAggregator>().Use(c => c.GetInstance<PluginRuntime>().EventAggregator);

			For<IActivityLogPathProvider>().Singleton().Use<ActivityLogPathProvider>();
			For<IActivityLogger>().Singleton().Use(CreateActivityLogger);
			For<ILog4NetFileRepository>().Singleton().Use<Log4NetFileRepository>();
		}

		protected virtual IActivityLogger CreateActivityLogger()
		{
			return ObjectFactory.GetInstance<PluginActivityLogger>();
		}

		protected virtual IAssembliesHost GetAssembliesHost()
		{
			return new AssembliesHost();
		}

		protected virtual IProfileStoragePersister GetProfileStoragePersisterInstance()
		{
			return ObjectFactory.GetInstance<ProfileStorageSqlPersister>();
		}

		protected virtual IPluginPersister GetPluginPersisterInstance()
		{
			return ObjectFactory.GetInstance<PluginPersister>();
		}

		protected virtual IAccountPersister GetAccountPersisterInstance()
		{
			return ObjectFactory.GetInstance<AccountPersister>();
		}

		protected virtual IProfilePersister GetProfilePersisterInstance()
		{
			return ObjectFactory.GetInstance<ProfilePersister>();
		}


		private void ConfigureWebServices()
		{
			For<AssignableService>().Use(CreateService<AssignableService>);
			For<AttachmentService>().Use(CreateService<AttachmentService>);
			For<AuditHistoryService>().Use(CreateService<AuditHistoryService>);
			For<AuthenticationService>().Use(CreateService<AuthenticationService>);
			For<BugService>().Use(CreateService<BugService>);
			For<BuildService>().Use(CreateService<BuildService>);
			For<CommentService>().Use(CreateService<CommentService>);
			For<CustomActivityService>().Use(CreateService<CustomActivityService>);
			For<CustomFieldService>().Use(CreateService<CustomFieldService>);
			For<EntityStateService>().Use(CreateService<EntityStateService>);
			For<FeatureService>().Use(CreateService<FeatureService>);
			For<FileService>().Use(CreateService<FileService>);
			For<GeneralService>().Use(CreateService<GeneralService>);
			For<GeneralUserService>().Use(CreateService<GeneralUserService>);
			For<ImpedimentService>().Use(CreateService<ImpedimentService>);
			For<IterationService>().Use(CreateService<IterationService>);
			For<MyAssignmentsService>().Use(CreateService<MyAssignmentsService>);
			For<PasswordRecoveryService>().Use(CreateService<PasswordRecoveryService>);
			For<PriorityService>().Use(CreateService<PriorityService>);
			For<ProcessService>().Use(CreateService<ProcessService>);
			For<ProgramService>().Use(CreateService<ProgramService>);
			For<ProjectMemberService>().Use(CreateService<ProjectMemberService>);
			For<ProjectService>().Use(CreateService<ProjectService>);
			For<ReleaseService>().Use(CreateService<ReleaseService>);
			For<RequesterService>().Use(CreateService<RequesterService>);
			For<RequestService>().Use(CreateService<RequestService>);
			For<RequestTypeService>().Use(CreateService<RequestTypeService>);
			For<RevisionFileService>().Use(CreateService<RevisionFileService>);
			For<RevisionService>().Use(CreateService<RevisionService>);
			For<RoleEffortService>().Use(CreateService<RoleEffortService>);
			For<RoleService>().Use(CreateService<RoleService>);
			For<SeverityService>().Use(CreateService<SeverityService>);
			For<SolutionService>().Use(CreateService<SolutionService>);
			For<TagsService>().Use(CreateService<TagsService>);
			For<TaskService>().Use(CreateService<TaskService>);
			For<TestCaseRunService>().Use(CreateService<TestCaseRunService>);
			For<TestCaseService>().Use(CreateService<TestCaseService>);
			For<TestPlanRunService>().Use(CreateService<TestPlanRunService>);
			For<TestPlanService>().Use(CreateService<TestPlanService>);
			For<TimeService>().Use(CreateService<TimeService>);
			For<UserService>().Use(CreateService<UserService>);
			For<UserStoryService>().Use(CreateService<UserStoryService>);
		}

		private static TService CreateService<TService>() where TService : WebServicesClientProtocol, new()
		{
			return ObjectFactory.GetInstance<ServiceManager>().GetService<TService>();
		}
	}
}
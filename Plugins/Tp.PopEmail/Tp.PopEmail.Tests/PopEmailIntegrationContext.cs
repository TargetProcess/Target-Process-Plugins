// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Messages.EntityLifecycle.Queries;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Testing.Common;
using Tp.Plugin.Core;
using Tp.PopEmailIntegration.Data;

namespace Tp.PopEmailIntegration
{
	public class PopEmailIntegrationContext
	{
		public PopEmailIntegrationContext()
		{
			var transportMock = TransportMock.CreateWithoutStructureMapClear(typeof (ProjectEmailProfile).Assembly,
			                                                                 new List<Assembly>
			                                                                 	{typeof (ExceptionThrownLocalMessage).Assembly},
			                                                                 new Assembly[] {});
			ObjectFactory.Configure(x => x.For<TransportMock>().Use(transportMock));

			ObjectFactory.Configure(x => x.For<PopEmailIntegrationContext>().Use(this));

			Transport.On<RetrieveAllUsersQuery>().Reply(x => new UserQueryResult {QueryResultCount = 0});
			Transport.On<RetrieveAllProjectsQuery>().Reply(x => new ProjectQueryResult {QueryResultCount = 0});
			Transport.On<RetrieveAllRequestersQuery>().Reply(x => new RequesterQueryResult {QueryResultCount = 0});
			Transport.On<RetrieveAllMessageUidsQuery>().Reply(x => new MessageUidQueryResult {QueryResultCount = 0});
			Transport.On<RetrieveGlobalSettingQuery>().Reply(x => new GlobalSettingQueryResult{Dtos =  new []{new GlobalSettingDTO {SMTPSender = Guid.NewGuid().ToString()}}, QueryResultCount = 1});

			Profile = Transport.AddProfile("Profile_1", GetProfileSettings());
		}

		public IProfileReadonly Profile { get; private set; }

		private static ProjectEmailProfile GetProfileSettings()
		{
			return new ProjectEmailProfile
			       	{
			       		Login = "login", 
						MailServer = "server", 
						Password = "pass", 
						Port = 2, 
						Protocol = "pop3", 
						Rules = "then attach to project 100 and create request in project 100",
						UseSSL = true
			       	};
		}

		public TransportMock Transport
		{
			get { return ObjectFactory.GetInstance<TransportMock>(); }
		}

		public IProfileReadonly Storage
		{
			get { return ObjectFactory.GetInstance<IProfileReadonly>(); }
		}

		public UserRepository UserRepository
		{
			get { return ObjectFactory.GetInstance<UserRepository>(); }
		}

		public MessageDTO Message { get; set; }

		public string TargetProcessEmail
		{
			set
			{
				var globalSettings = Profile.Get<GlobalSettingDTO>();
				var globalSetting = globalSettings.Single();
				globalSetting.SMTPSender = value;
				globalSettings.Clear();
				globalSettings.Add(globalSetting);
			}
		}
	}
}

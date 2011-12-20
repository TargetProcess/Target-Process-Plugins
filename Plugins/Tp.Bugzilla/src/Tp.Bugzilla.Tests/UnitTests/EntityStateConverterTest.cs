// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using Tp.Bugzilla.BugFieldConverters;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Mapping;
using Tp.Testing.Common.NUnit;

namespace Tp.Bugzilla.Tests.UnitTests
{
	[TestFixture]
	public class EntityStateConverterTest
	{
		private BugzillaContext _context;
		private EntityStateConverter _converter;

		private IStorageRepository StorageRepository
		{
			get { return ObjectFactory.GetInstance<IProfileReadonly>(); }
		}

		[SetUp]
		public void Init()
		{
			_context = ObjectFactory.GetInstance<BugzillaContext>();
			_context.Initialize();

			_context.AddProfile(1);

			StorageRepository.GetProfile<BugzillaProfile>().StatesMapping = new MappingContainer
			                                                                	{
			                                                                		new MappingElement
			                                                                			{
			                                                                				Key = "OPEN",
			                                                                				Value = new MappingLookup {Id = 1, Name = "Open"}
			                                                                			},
			                                                                		new MappingElement
			                                                                			{
			                                                                				Key = "Done",
			                                                                				Value =
			                                                                					new MappingLookup {Id = 2, Name = "Closed"}
			                                                                			}
			                                                                	};

			StorageRepository.Get<EntityStateDTO>().AddRange(new[]
			                                                 	{
			                                                 		new EntityStateDTO {ID = 1, Name = "Open"},
			                                                 		new EntityStateDTO {ID = 2, Name = "Closed"},
			                                                 		new EntityStateDTO {ID = 3, Name = "In Progress"},
			                                                 		new EntityStateDTO {ID = 4, Name = "Invalid"}
			                                                 	});

			var service = MockRepository.GenerateStub<IBugzillaService>();
			service.Stub(s => s.GetStatuses()).Return(new List<string> {"OPEN", "Done", "In Progress"});

			_converter = new EntityStateConverter(StorageRepository, service, ObjectFactory.GetInstance<IActivityLogger>());
		}

		[Test]
		public void ShouldGetMappingFromMap()
		{
			CheckMappedBugzillaValue(2, "Done");
		}

		[Test]
		public void ShouldGetMappingFromStorage()
		{
			CheckMappedBugzillaValue(3, "In Progress");
		}

		[Test]
		public void ShouldReturnNullIfNoMappingAndNotInStorage()
		{
			CheckMappedBugzillaValue(4, null);
		}

		private void CheckMappedBugzillaValue(int tpEntityStateId, string bzStatus)
		{
			var mapped = _converter.GetMappedBugzillaStatus(new BugDTO {EntityStateID = tpEntityStateId});
			mapped.Should(Be.EqualTo(bzStatus));
		}
	}
}
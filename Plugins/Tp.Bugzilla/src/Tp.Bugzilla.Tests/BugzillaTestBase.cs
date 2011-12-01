// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Linq;
using NUnit.Framework;
using StructureMap;
using Tp.Integration.Common;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Testing.Common;

namespace Tp.Bugzilla.Tests
{
	public class BugzillaTestBase
	{
		[SetUp]
		public virtual void Init()
		{
			ObjectFactory.Initialize(x => x.AddRegistry<BugzillaContextRegistry>());
			ObjectFactory.GetInstance<BugzillaContext>().Initialize();
		}

		protected BugzillaContext Context
		{
			get { return ObjectFactory.GetInstance<BugzillaContext>(); }
		}

		protected static IProfileReadonly Profile
		{
			get { return ObjectFactory.GetInstance<IProfileReadonly>(); }
		}

		protected TransportMock TransportMock
		{
			get { return ObjectFactory.GetInstance<TransportMock>(); }
		}

		protected BugDTO GetBug(string bugName)
		{
			return Context.TpBugs.Single(x => x.Name == bugName);
		}

		protected BugDTO GetBug(int bugId)
		{
			return Context.TpBugs.Single(x => x.ID == bugId);
		}
	}
}

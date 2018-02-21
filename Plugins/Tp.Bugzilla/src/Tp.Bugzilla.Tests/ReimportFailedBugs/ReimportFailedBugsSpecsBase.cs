// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using NBehave.Narrator.Framework;
using StructureMap;
using Tp.Bugzilla.Schemas;
using Tp.Bugzilla.Tests.Mocks;
using Tp.Bugzilla.Tests.Synchronization;
using Tp.Integration.Messages.Ticker;
using Tp.Integration.Plugin.Common.Storage;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla.Tests.ReimportFailedBugs
{
    [ActionSteps]
    public class ReimportFailedBugsSpecsBase<T> : BugzillaTestBase
        where T : class, IBugzillaServiceFailMock, IBugzillaService
    {
        public override void Init()
        {
            base.Init();

            ObjectFactory.Configure(x =>
            {
                x.For<IBugzillaService>().HybridHttpOrThreadLocalScoped().Use<T>();
                x.Forward<IBugzillaService, T>();

                x.For<IBugzillaServiceFailMock>().HybridHttpOrThreadLocalScoped().Use<T>();
                x.Forward<IBugzillaServiceFailMock, T>();
            });
        }

        protected void FailSynchronization()
        {
            ObjectFactory.GetInstance<IBugzillaServiceFailMock>().Fail = true;
            ((IBugzillaServiceFailMock) ObjectFactory.GetInstance<IBugzillaService>()).Fail = true;
            new BugSyncActionSteps().SynchronizeBugs();
        }

        [Given("bugzilla contains bug $id and name '$name' created on '$created'")]
        public void CreateBugzillaBug(int id, string name, string created)
        {
            Context.BugzillaBugs.Add(new bug
            {
                bug_id = id.ToString(),
                short_desc = name,
                creation_ts = DateTime.Parse(created).ToString("dd MMM yyyy HH':'mm")
            });
        }

        [Given("last synchronization date is '$lastSyncDate'")]
        public void SetLastSyncDate(string lastSyncDate)
        {
            ObjectFactory.GetInstance<IStorageRepository>().Get<LastSyncDate>().ReplaceWith(
                new LastSyncDate(DateTime.Parse(lastSyncDate)));
        }
    }
}

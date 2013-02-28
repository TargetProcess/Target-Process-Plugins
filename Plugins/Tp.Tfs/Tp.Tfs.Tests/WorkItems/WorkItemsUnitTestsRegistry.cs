using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Rhino.Mocks;
using StructureMap.Configuration.DSL;
using Tp.Integration.Plugin.Common.PluginLifecycle;
using Tp.Integration.Testing.Common;
using Tp.Plugin.Core;
using Tp.SourceControl.Comments;

namespace Tp.Tfs.Tests.WorkItems
{
    public class WorkItemsUnitTestsRegistry : Registry
    {
        public WorkItemsUnitTestsRegistry()
        {
            For<TransportMock>().Use(
                TransportMock.CreateWithoutStructureMapClear(
                    typeof(TfsPluginProfile).Assembly,
                    new List<Assembly>
                    {
                        typeof (ExceptionThrownLocalMessage).Assembly,
                        typeof (TfsPluginProfile).Assembly
                    },
                    new Assembly[] { }));
        }
    }
}

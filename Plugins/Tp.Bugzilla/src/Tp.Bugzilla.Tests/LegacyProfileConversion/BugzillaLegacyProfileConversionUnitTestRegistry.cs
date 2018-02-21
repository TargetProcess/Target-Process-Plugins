//
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
//

using System.Collections.Generic;
using System.Reflection;
using Rhino.Mocks;
using Tp.Integration.Plugin.Common;
using Tp.Integration.Plugin.Common.PluginLifecycle;
using Tp.Integration.Plugin.Common.Storage.Persisters;
using Tp.Integration.Testing.Common;
using Tp.LegacyProfileConversion.Common.Testing;
using Tp.Plugin.Core;

namespace Tp.Bugzilla.Tests.LegacyProfileConversion
{
    public class BugzillaLegacyProfileConversionUnitTestRegistry : LegacyProfileConverterUnitTestRegistry
    {
        public BugzillaLegacyProfileConversionUnitTestRegistry()
        {
            var stub = MockRepository.GenerateStub<IDatabaseConfiguration>();
            stub.Expect(s => s.ConnectionString).Return("Data Source=(local);Initial Catalog=TargetProcessTest;Integrated Security=SSPI");

            For<IDatabaseConfiguration>().Use(stub);

            For<TransportMock>().Use(TransportMock.CreateWithoutStructureMapClear(typeof(BugzillaProfile).Assembly,
                new List<Assembly>
                {
                    typeof(ExceptionThrownLocalMessage).Assembly,
                    typeof(BugzillaProfile).Assembly
                },
                new Assembly[] { }));

            For<IAssembliesHost>().Singleton().Use(new PredefinedAssembliesHost(new[] { typeof(BugzillaProfile).Assembly }));

            var mock = MockRepository.GenerateMock<PluginInitializer>();
            mock.Stub(x => x.SendInfoMessages());
            For<PluginInitializer>().Use(mock);

            IncludeRegistry<PluginRegistry>();
        }

        protected override Assembly PluginAssembly
        {
            get { return typeof(BugzillaProfile).Assembly; }
        }
    }
}

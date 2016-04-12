// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections.Generic;
using System.Reflection;
using Rhino.Mocks;
using Tp.Integration.Plugin.Common.PluginLifecycle;
using Tp.Integration.Testing.Common;
using Tp.LegacyProfileConversion.Common.Testing;
using Tp.Plugin.Core;

namespace Tp.Tfs.Tests.LegacyProfileConversionFeature
{
	public class TfsLegacyProfileConversionUnitTestRegistry : LegacyProfileConverterUnitTestRegistry
	{
        public TfsLegacyProfileConversionUnitTestRegistry()
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

            var mock = MockRepository.GenerateMock<PluginInitializer>();
            mock.Stub(x => x.SendInfoMessages());
            For<PluginInitializer>().Use(mock);
        }

		protected override Assembly PluginAssembly
		{
			get { return typeof (TfsPluginProfile).Assembly; }
		}
	}
}
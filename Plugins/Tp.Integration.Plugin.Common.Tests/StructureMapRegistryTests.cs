// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using NUnit.Framework;
using StructureMap;
using Tp.Integration.Plugin.Common.Tests.Common.PluginCommand;
using Tp.Integration.Testing.Common;

namespace Tp.Integration.Plugin.Common.Tests
{
    [TestFixture]
    [Category("PartPlugins1")]
    public class StructureMapRegistryTests
    {
        [SetUp]
        public void Init()
        {
            //This configures structureMap inside.
            TransportMock.CreateWithoutStructureMapClear(typeof(WhenAddANewProfileSpecs).Assembly,
                typeof(WhenAddANewProfileSpecs).Assembly);
        }

        [Test]
        public void CheckConfiguration()
        {
            ObjectFactory.AssertConfigurationIsValid();
        }
    }
}

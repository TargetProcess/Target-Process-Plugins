// 
// Copyright (c) 2005-2017 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Tp.Integration.Plugin.TestRunImport.FrameworkTypes;
using Tp.TestRunImport.Tests.FtpIntegration;
using Tp.TestRunImport.Tests.HttpIntegration;

namespace Tp.TestRunImport.Tests
{
    [TestFixture]
    public abstract class CreateTestPlanRunSagaSpecsBase
    {
        private readonly DirectoryInfo _dir;
        private FtpIntegrationServer _ftpServer;
        private HttpIntegrationServer _webServer;

        protected CreateTestPlanRunSagaSpecsBase()
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            _dir = new DirectoryInfo(new Uri(string.Format($"{directoryName}\\{FrameworkType}")).AbsolutePath);
        }

        [TestFixtureSetUp]
        public void SetUp()
        {
            _ftpServer = new FtpIntegrationServer(_dir.FullName, 2121);
            _webServer = new HttpIntegrationServer(_dir.FullName, 2123);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _ftpServer?.Dispose();
            _webServer?.Dispose();
        }

        protected DirectoryInfo FtpDirectory => _dir;

        protected abstract FrameworkTypes FrameworkType { get; }
    }
}

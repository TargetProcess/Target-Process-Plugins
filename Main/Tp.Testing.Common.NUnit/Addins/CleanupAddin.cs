// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
//using NUnit.Core;
//using NUnit.Core.Extensibility;

//namespace Tp.Testing.Common.NUnit.Addins
//{
//    [NUnitAddin(Name = "CleanupAddin", Description = "Performs cleanup after test fixture execution",
//        Type = ExtensionType.Core)]
//    public sealed class CleanupAddin : IAddin, ITestDecorator, EventListener
//    {
//        private static readonly object SyncRoot = new object();
//        private readonly List<NUnitTestFixture> _fixtureList = new List<NUnitTestFixture>();

//        #region IAddin Members

//        public bool Install(IExtensionHost host)
//        {
//            host.GetExtensionPoint("EventListeners").Install(this);
//            host.GetExtensionPoint("TestDecorators").Install(this);

//            return true;
//        }

//        #endregion

//        #region ITestDecorator Members

//        public Test Decorate(Test test, MemberInfo member)
//        {
//            if (test is NUnitTestFixture)
//            {
//                lock (SyncRoot)
//                {
//                    _fixtureList.Add((NUnitTestFixture) test);
//                }
//            }

//            return test;
//        }

//        #endregion

//        #region EventListener Members

//        public void RunStarted(string name, int testCount)
//        {
//        }

//        public void RunFinished(TestResult result)
//        {
//        }

//        public void TestFinished(TestResult result)
//        {
//            var fixture = FindCurrentFixture(result);
//            if (fixture == null)
//            {
//                Console.WriteLine("Failed to find test fixture for test: {0}", result.Test.TestName);
//                return;
//            }

//            var methods = Reflect.GetMethodsWithAttribute(fixture.FixtureType, typeof (CleanupOnErrorAttribute).FullName, true);
//            foreach (var method in methods)
//            {
//                try
//                {
//                    method.Invoke(Activator.CreateInstance(fixture.FixtureType), new object[] {result});
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine("Failed to invoke CleanOnError method: {0}. Error acquired: {1}", method.Name, ex.Message);
//                }
//            }
//        }

//        private NUnitTestFixture FindCurrentFixture(TestResult result)
//        {
//            return _fixtureList.FirstOrDefault(fixture => result.FullName.Contains(fixture.FixtureType.FullName));
//        }

//        public void RunFinished(Exception exception)
//        {
//        }

//        public void TestStarted(TestName testName)
//        {
//        }

//        public void SuiteStarted(TestName testName)
//        {
//        }

//        public void SuiteFinished(TestResult result)
//        {
//        }

//        public void UnhandledException(Exception exception)
//        {
//        }

//        public void TestOutput(TestOutput testOutput)
//        {
//        }

//        #endregion
//    }
//}
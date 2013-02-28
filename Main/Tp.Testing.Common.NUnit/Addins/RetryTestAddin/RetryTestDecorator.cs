using System;
using System.Collections;
using System.Diagnostics;
//using NUnit.Core;
using NUnit.Framework;

namespace Tp.Testing.Common.NUnit.Addins.RetryTestAddin
{/*
    [TestFixture]
    public class Tests
    {
        private static bool setup;
        private static bool res;

		[SetUp]
		public void SetUp()
		{
			Console.WriteLine("SetUp called");
			if (!setup)
			{
				setup = true;
				//throw new Exception();
			}
		}

		[TearDown]
		public void TearDown()
		{
			Console.WriteLine("TearDown called");
		}

		[Test]
        public void RetryTest()
        {
            if (!res)
            {
                res = true;
                Console.WriteLine();
                throw new Exception();
            }
            Console.WriteLine("OK");
        }
    }

    public class RetryTestDecorator : Test
    {
        static readonly Logger Log = InternalTrace.GetLogger("RetryTestDecorator");

        private int retryCount;
        private Test test;
        public RetryTestDecorator(Test test, int retryCount)
            : base(test.TestName)
        {
            this.test = test;
			this.RunState = test.RunState;
            this.Categories = test.Categories;
            this.retryCount = retryCount;
        }

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
        	test.Parent = Parent;

			TestResult result = null;
			for (var i = 1; i < retryCount; i++)
            {
            	try
            	{
            		result = test.Run(listener, filter);
            		if (result.IsFailure || result.IsError)
            		{
            			var message = string.Format("Rerunning '{0}'", TestName);
            			Log.Info(message); Console.WriteLine(message);
            			result = test.Run(listener, filter);
            		}
            	}
				// In Case SetUp or TearDown failed
				catch (Exception e)
            	{
					Log.Info(e.Message); Console.WriteLine(e);
            	}
            }

            return result;
        }

		public override int CountTestCases(ITestFilter filter)
        {
            return test.CountTestCases(filter);
        }

        public override int TestCount
        {
            get { return test.TestCount; }
        }

        public override bool IsSuite
        {
            get { return test.IsSuite; }
        }

        public override IList Tests
        {
            get { return test.Tests; }
        }

        public override Type FixtureType
        {
            get { return test.FixtureType; }
        }

        public override string TestType
        {
            get { return test.TestType; }
        }

        public override object Fixture
        {
            get { return test.Fixture; }
            set { test.Fixture = value; }
        }
    }

*/}

using System.Reflection;
//using NUnit.Core;
//using NUnit.Core.Extensibility;

namespace Tp.Testing.Common.NUnit.Addins.RetryTestAddin
{/*
    [NUnitAddin(Description = "Retry to run test", Type = ExtensionType.Core)]
    public class RetryTestAddin : IAddin,  ITestDecorator
    {
        #region IAddin Members
        static readonly Logger Log = InternalTrace.GetLogger("RetryTestAddin");

        public bool Install(IExtensionHost host)
        {

            Log.Info("RetryTestAddin: Install called");
            var decorators = host.GetExtensionPoint("TestDecorators");
            if (decorators == null) return false;

            decorators.Install(this);
            return true;
        }

        #endregion

        #region ITestDecorator Members

        public Test Decorate(Test test, MemberInfo member)
        {
            if (test is TestMethod)
            {
//                var attr = Reflect.GetAttribute(
//                    member, "Tp.Testing.Common.NUnit.Addins.RetryTestAddin.RetryTestAttribute", false) as RetryTestAttribute;
//
//                if (attr != null)
//                {
//                    test = new RetryTestDecorator(test, attr.RetryCount);
//                    Log.Info(string.Format("Decorated: '{0}'", member.Name));
//                }
                test = new RetryTestDecorator(test, 2);
            }
            
            return test;
        }

        #endregion
    }
*/}

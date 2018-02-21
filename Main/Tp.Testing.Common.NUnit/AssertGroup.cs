using System;
using System.Text;
using NUnit.Framework;

namespace Tp.Testing.Common.NUnit
{
    public static class AssertGroup
    {
        public static void AssertMultiple(params Action[] assertions)
        {
            StringBuilder messages = new StringBuilder();

            foreach (var assertion in assertions)
            {
                try
                {
                    assertion();
                }
                catch (Exception e)
                {
                    messages.Append($"\n{e.Message}");
                }
            }

            if (messages.Length > 0)
            {
                Assert.Fail(messages.ToString());
            }
        }
    }
}

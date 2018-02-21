using System;

namespace NUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class BugAttribute : TestAttribute
    {
        public BugAttribute(int id)
        {
            ID = id;
            Description = "Test for Bug #" + id;
        }

        public int ID { get; }
    }
}

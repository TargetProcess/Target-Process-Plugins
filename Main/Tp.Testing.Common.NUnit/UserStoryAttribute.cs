using System;

namespace NUnit.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class UserStoryAttribute : TestAttribute
    {
        public UserStoryAttribute(int id)
        {
            ID = id;
            Description = "Test for User Story #" + id;
        }

        public int ID { get; }
    }
}

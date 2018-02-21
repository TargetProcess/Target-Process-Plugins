using System;

namespace Tp.Core.Annotations
{
    public class PerformanceCriticalAttribute : Attribute
    {
        private readonly string _description;

        public PerformanceCriticalAttribute(string description = null)
        {
            _description = description;
        }
    }
}

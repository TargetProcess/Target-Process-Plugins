using System;

namespace Tp.Utils.Documentation
{
    public class ApiParameter
    {
        public ApiParameter(string name, Type type, string description)
        {
            Name = name;
            Type = type;
            Description = description;
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public string Description { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tp.Utils.Documentation
{
    public class ApiDataType
    {
        public ApiDataType(Type type, string description, IList<ApiParameter> properties)
        {
            Type = type;
            Description = description;
            Properties = new ReadOnlyCollection<ApiParameter>(properties);
        }

        public Type Type { get; private set; }

        public string Description { get; private set; }

        public ReadOnlyCollection<ApiParameter> Properties { get; private set; }
    }
}

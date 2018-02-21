using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tp.Utils.Documentation
{
    public class ApiAction<TParameter>
    {
        public ApiAction(
            string uri,
            string name,
            string methodType,
            string description,
            bool isObsolete,
            TParameter returnValue, IList<TParameter> parameters)
        {
            Uri = uri;
            Name = name;
            MethodType = methodType;
            Description = description;
            IsObsolete = isObsolete;
            ReturnValue = returnValue;
            Parameters = new ReadOnlyCollection<TParameter>(parameters);
        }

        public string Uri { get; private set; }

        public string Name { get; private set; }

        public string MethodType { get; private set; }

        public string Description { get; private set; }

        public bool IsObsolete { get; private set; }

        public TParameter ReturnValue { get; private set; }

        public ReadOnlyCollection<TParameter> Parameters { get; private set; }
    }
}

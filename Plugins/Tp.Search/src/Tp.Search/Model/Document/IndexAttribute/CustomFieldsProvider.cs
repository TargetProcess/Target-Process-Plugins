using System;
using System.Collections.Generic;
using System.Linq;

namespace Tp.Search.Model.Document.IndexAttribute
{
    class CustomFieldsProvider
    {
        public static readonly CustomFieldsProvider Instance = new CustomFieldsProvider();

        private CustomFieldsProvider()
        {
        }

        public IEnumerable<Enum> GetCustomFields<TField>()
        {
            return Enumerable.Range(1, 60).Select(i => "CustomField" + i).Select(n => (Enum) Enum.Parse(typeof(TField), n));
        }
    }
}

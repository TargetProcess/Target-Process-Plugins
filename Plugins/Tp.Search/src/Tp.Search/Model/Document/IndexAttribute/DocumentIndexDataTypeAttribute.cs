using System;
using System.Collections.Generic;

namespace Tp.Search.Model.Document.IndexAttribute
{
    [AttributeUsage(AttributeTargets.Field)]
    class DocumentIndexDataTypeAttribute : Attribute
    {
        private readonly IEnumerable<DocumentIndexDataTypeToken> _tokens;

        public DocumentIndexDataTypeAttribute(DocumentIndexDataTypeToken[] tokens)
        {
            _tokens = tokens;
        }

        public IEnumerable<DocumentIndexDataTypeToken> Tokens
        {
            get { return _tokens; }
        }
    }
}

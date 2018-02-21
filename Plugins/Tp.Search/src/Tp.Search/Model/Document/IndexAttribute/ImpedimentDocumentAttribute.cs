// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Integration.Common;

namespace Tp.Search.Model.Document.IndexAttribute
{
    [AttributeUsage(AttributeTargets.Field)]
    class ImpedimentDocumentAttribute : Attribute, IDocumentFieldsProvider
    {
        private readonly IEnumerable<Enum> _fields;

        public ImpedimentDocumentAttribute(ImpedimentField[] fields)
        {
            _fields = fields.Cast<Enum>().ToList();
        }

        public IEnumerable<Enum> DocumentFields
        {
            get { return _fields; }
        }
    }
}

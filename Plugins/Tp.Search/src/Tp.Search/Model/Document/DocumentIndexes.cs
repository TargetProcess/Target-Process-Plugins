// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Concurrent;

namespace Tp.Search.Model.Document
{
    class DocumentIndexes
    {
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _entityIndexes;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _commentIndexes;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _entityProjectIndexes;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _commentProjectIndexes;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _entityTypeIndexes;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _entiyStateIndexes;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _entitySquadIndexes;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _commentSquadIndexes;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _commentEntityTypes;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _impedimentContextIndexes;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _testStepIndexes;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentIndex>> _testStepProjectIndexes;

        public DocumentIndexes()
        {
            _entityIndexes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
            _commentIndexes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
            _entityProjectIndexes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
            _commentProjectIndexes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
            _entityTypeIndexes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
            _entiyStateIndexes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
            _entitySquadIndexes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
            _commentSquadIndexes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
            _commentEntityTypes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
            _impedimentContextIndexes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
            _testStepIndexes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
            _testStepProjectIndexes = new ConcurrentDictionary<string, Lazy<IDocumentIndex>>();
        }

        public ConcurrentDictionary<string, Lazy<IDocumentIndex>> this[DocumentIndexTypeToken documentIndexTypeToken]
        {
            get
            {
                switch (documentIndexTypeToken)
                {
                    case DocumentIndexTypeToken.Entity:
                        return _entityIndexes;
                    case DocumentIndexTypeToken.Comment:
                        return _commentIndexes;
                    case DocumentIndexTypeToken.EntityProject:
                        return _entityProjectIndexes;
                    case DocumentIndexTypeToken.CommentProject:
                        return _commentProjectIndexes;
                    case DocumentIndexTypeToken.EntityType:
                        return _entityTypeIndexes;
                    case DocumentIndexTypeToken.EntityState:
                        return _entiyStateIndexes;
                    case DocumentIndexTypeToken.EntitySquad:
                        return _entitySquadIndexes;
                    case DocumentIndexTypeToken.CommentSquad:
                        return _commentSquadIndexes;
                    case DocumentIndexTypeToken.CommentEntityType:
                        return _commentEntityTypes;
                    case DocumentIndexTypeToken.Impediment:
                        return _impedimentContextIndexes;
                    case DocumentIndexTypeToken.TestStep:
                        return _testStepIndexes;
                    case DocumentIndexTypeToken.TestStepProject:
                        return _testStepProjectIndexes;
                    default:
                        throw new NotSupportedException("{0} is not supported.".Fmt(documentIndexTypeToken));
                }
            }
        }
    }
}

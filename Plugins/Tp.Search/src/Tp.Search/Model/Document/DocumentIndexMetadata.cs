using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Search.Model.Document.IndexAttribute;

namespace Tp.Search.Model.Document
{
	class DocumentIndexMetadata
	{
		private readonly IDictionary<IndexTypeToken, DocumentIndexType> _documentIndexTypes;
		public DocumentIndexMetadata()
		{
			_documentIndexTypes = Load(new DocumentIndexDataTypeServiceFactory());
		}

		public IEnumerable<DocumentIndexType> DocumentIndexTypes
		{
			get { return _documentIndexTypes.Values; }
		}

		public DocumentIndexType GetDocumentIndexType(DocumentIndexTypeToken typeToken, DocumentIndexDataTypeToken dataTypeToken)
		{
			return _documentIndexTypes[new IndexTypeToken{Type = typeToken, DataType = dataTypeToken}];
		}

		public bool Contains(DocumentIndexTypeToken typeToken, DocumentIndexDataTypeToken dataTypeToken)
		{
			return _documentIndexTypes.Keys.Contains(new IndexTypeToken
				{
					Type = typeToken,
					DataType = dataTypeToken
				});
		}

		private static IDictionary<IndexTypeToken, DocumentIndexType> Load(DocumentIndexDataTypeServiceFactory documentIndexDataTypeServiceFactory)
		{
			IDictionary<DocumentIndexTypeToken, IndexFileName> typeDescriptions = EnumServices.Load<DocumentIndexTypeToken, IndexFileName>();
			var documentIndexAttribute = typeof(DocumentIndexType).GetCustomAttribute<DocumentIndexAttribute>()
			                                                      .GetOrThrow(() => new InvalidOperationException("DocumentIndexType type should have DocumentIndexAttribute"));
			var accSource = Enum.GetValues(typeof(DocumentIndexTypeToken))
			                    .Cast<DocumentIndexTypeToken>()
			                    .ToDictionary(x => x, _ => new DocumentIndexTypeTokenMetadata());
			return documentIndexAttribute.IndexAttributeTypes.Concat(documentIndexAttribute.DocumentAttributeTypes)
			                             .Concat(new[] {documentIndexAttribute.VersionAttributeType, documentIndexAttribute.DataTypeTokenAttributeType})
			                             .Select(EnumServices.Load<DocumentIndexTypeToken>)
			                             .Aggregate(accSource, (acc, x) =>
				                             {
					                             foreach (KeyValuePair<DocumentIndexTypeToken, Attribute> p in x)
					                             {
						                             var metadata = acc[p.Key];
						                             var indexableFieldsProvider = p.Value as IIndexFieldsProvider;
						                             if (indexableFieldsProvider != null)
						                             {
							                             metadata.AddIndexFields(indexableFieldsProvider.IndexFields);
						                             }
						                             var documentFieldsProvider = p.Value as IDocumentFieldsProvider;
						                             if (documentFieldsProvider != null)
						                             {
							                             metadata.AddDocumentFields(documentFieldsProvider.DocumentFields);
						                             }
						                             var maybeVersion = p.Value as DocumentIndexVersionAttribute;
						                             if (maybeVersion != null)
						                             {
							                             metadata.Version = maybeVersion.Version;
						                             }
						                             var maybeDataToken = p.Value as DocumentIndexDataTypeAttribute;
						                             if (maybeDataToken != null)
						                             {
							                             metadata.AddDataTypeToken(maybeDataToken.Tokens);
						                             }
					                             }
					                             return acc;
				                             })
										.SelectMany(p => p.Value.DataTypeTokens.Select(d => new DocumentIndexType(p.Key, d, typeDescriptions[p.Key].FileName, p.Value.IndexFields, p.Value.DocumentFields, p.Value.Version, documentIndexDataTypeServiceFactory.Create(d), new FileService())))
			                             .ToDictionary(x => new IndexTypeToken{Type = x.TypeToken, DataType = x.DataTypeToken}, _ => _);
		}

		private class DocumentIndexTypeTokenMetadata
		{
			private readonly List<Enum> _indexFields;
			private readonly List<Enum> _documentFields;
			private readonly List<DocumentIndexDataTypeToken> _dataTypeTokens;
			public DocumentIndexTypeTokenMetadata()
			{
				_indexFields = new List<Enum>();
				_documentFields = new List<Enum>();
				_dataTypeTokens = new List<DocumentIndexDataTypeToken>();
			}

			public void AddIndexFields(IEnumerable<Enum> fields)
			{
				_indexFields.AddRange(fields);
			}

			public void AddDocumentFields(IEnumerable<Enum> fields)
			{
				_documentFields.AddRange(fields);
			}

			public void AddDataTypeToken(IEnumerable<DocumentIndexDataTypeToken> dataTypeTokens)
			{
				_dataTypeTokens.AddRange(dataTypeTokens);
			}

			public IEnumerable<Enum> IndexFields { get { return _indexFields; } }
			public IEnumerable<Enum> DocumentFields { get { return _documentFields; } }
			public int Version { get; set; }
			public IEnumerable<DocumentIndexDataTypeToken> DataTypeTokens
			{
				get { return _dataTypeTokens; }
			}
		}

		private struct IndexTypeToken : IEquatable<IndexTypeToken>
		{
			public DocumentIndexTypeToken Type { get; set; }
			public DocumentIndexDataTypeToken DataType { get; set; }

			public bool Equals(IndexTypeToken other)
			{
				return DataType == other.DataType && Type == other.Type;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				return obj is IndexTypeToken && Equals((IndexTypeToken)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return ((int)DataType * 397) ^ (int)Type;
				}
			}
		}
	}
}
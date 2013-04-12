using System;
using System.Collections.Generic;
using System.Linq;
using Tp.Core;
using Tp.Search.Model.Document.IndexAttribute;

namespace Tp.Search.Model.Document
{
	[DocumentIndex(indexAttributeTypes: new[] { typeof(GeneralIndexAttribute), typeof(AssignableIndexAttribute), typeof(TestCaseIndexAttribute), typeof(CommentIndexAttribute) },
				   documentAttributeTypes:new[]{typeof(GeneralDocumentAttribute), typeof(AssignableDocumentAttribute),typeof(TestCaseDocumentAttribute)},
				   versionAttributeType:typeof(DocumentIndexVersionAttribute))]
	class DocumentIndexType
	{
		private readonly DocumentIndexTypeToken _typeToken;
		private readonly string _fileName;
		private readonly IEnumerable<Enum> _indexFields;
		private readonly IEnumerable<Enum> _documentFields;
		private readonly int _version;
		private DocumentIndexType(DocumentIndexTypeToken typeToken, string fileName, IEnumerable<Enum> indexFields, IEnumerable<Enum> documentFields, int version)
		{
			_typeToken = typeToken;
			_fileName = fileName + "_v" + version;
			_indexFields = indexFields;
			_documentFields = documentFields;
			_version = version;
		}

		public DocumentIndexTypeToken TypeToken
		{
			get { return _typeToken; }
		}

		public string FileName
		{
			get { return _fileName; }
		}

		public int Version
		{
			get { return _version; }
		}

		public bool IsBelongedToIndexFields(Enum field)
		{
			return _indexFields.Any(f => f.Equals(field));
		}

		public bool IsBelongedToDocumentFields(Enum field)
		{
			return _documentFields.Any(f => f.Equals(field));
		}

		private class DocumentIndexTypeTokenMetadata
		{
			private readonly List<Enum> _indexFields;
			private readonly List<Enum> _documentFields;
			public DocumentIndexTypeTokenMetadata()
			{
				_indexFields = new List<Enum>();
				_documentFields = new List<Enum>();
			}

			public void AddIndexFields(IEnumerable<Enum> fields)
			{
				_indexFields.AddRange(fields);
			}

			public void AddDocumentFields(IEnumerable<Enum> fields)
			{
				_documentFields.AddRange(fields);
			}

			public IEnumerable<Enum> IndexFields { get { return _indexFields; } }
			public IEnumerable<Enum> DocumentFields { get { return _documentFields; } }
			public int Version { get; set; }
		}

		public static IDictionary<DocumentIndexTypeToken, DocumentIndexType> Load()
		{
			IDictionary<DocumentIndexTypeToken, IndexFileName> typeDescriptions = EnumServices.Load<DocumentIndexTypeToken, IndexFileName>();
			var documentIndexAttribute = typeof (DocumentIndexType).GetCustomAttribute<DocumentIndexAttribute>()
																														.FailIfNothing(() => new InvalidOperationException("DocumentIndexType type should have DocumentIndexAttribute"));
			var accSource = Enum.GetValues(typeof (DocumentIndexTypeToken))
													.Cast<DocumentIndexTypeToken>()
													.ToDictionary(x => x, _ => new DocumentIndexTypeTokenMetadata());
			return documentIndexAttribute.IndexAttributeTypes.Concat(documentIndexAttribute.DocumentAttributeTypes).Concat(new[] { documentIndexAttribute.VersionAttributeType })
																	.Select(EnumServices.Load<DocumentIndexTypeToken>)
																	.Aggregate(accSource, (acc, x) =>
				{
					foreach (KeyValuePair<DocumentIndexTypeToken, Attribute> p in x)
					{
						var indexableFieldsProvider = p.Value as IIndexFieldsProvider;
						if (indexableFieldsProvider != null)
						{
							acc[p.Key].AddIndexFields(indexableFieldsProvider.IndexFields);
						}
						var documentFieldsProvider = p.Value as IDocumentFieldsProvider;
						if (documentFieldsProvider != null)
						{
							acc[p.Key].AddDocumentFields(documentFieldsProvider.DocumentFields);
						}
						var maybeVersion = p.Value as DocumentIndexVersionAttribute;
						if(maybeVersion != null)
						{
							acc[p.Key].Version = maybeVersion.Version;
						}
					}
					return acc;
				}).ToDictionary(p => p.Key, p => new DocumentIndexType(p.Key, typeDescriptions[p.Key].FileName, p.Value.IndexFields, p.Value.DocumentFields, p.Value.Version));
		}
	}
}
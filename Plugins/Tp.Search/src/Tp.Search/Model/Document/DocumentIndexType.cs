// 
// Copyright (c) 2005-2014 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tp.Core;
using Tp.Integration.Messages;
using Tp.Search.Model.Document.IndexAttribute;
using hOOt;

namespace Tp.Search.Model.Document
{
	[DocumentIndex(indexAttributeTypes: new[] { typeof(GeneralIndexAttribute), typeof(AssignableIndexAttribute), typeof(TestCaseIndexAttribute), typeof(ImpedimentIndexAttribute), typeof(CommentIndexAttribute) },
								documentAttributeTypes: new[] { typeof(GeneralDocumentAttribute), typeof(AssignableDocumentAttribute), typeof(TestCaseDocumentAttribute), typeof(ImpedimentDocumentAttribute) },
								versionAttributeType: typeof(DocumentIndexVersionAttribute),
								dataTypeTokenAttributeType: typeof(DocumentIndexDataTypeAttribute))]
	class DocumentIndexType
	{
		private readonly DocumentIndexTypeToken _typeToken;
		private readonly DocumentIndexDataTypeToken _dataTypeToken;
		private readonly string _fileName;
		private readonly string _versionedFileName;
		private readonly IEnumerable<Enum> _indexFields;
		private readonly IEnumerable<Enum> _documentFields;
		private readonly int _version;
		private readonly DocumentIndexDataTypeService _documentIndexDataTypeService;
		private readonly IFileService _fileService;
		private static readonly string IndexFileRootPath;

		static DocumentIndexType()
		{
			IndexFileRootPath = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
		}

		public DocumentIndexType(DocumentIndexTypeToken typeToken, DocumentIndexDataTypeToken dataTypeToken, string fileName, IEnumerable<Enum> indexFields, IEnumerable<Enum> documentFields, int version, DocumentIndexDataTypeService documentIndexDataTypeService, IFileService fileService)
		{
			_typeToken = typeToken;
			_dataTypeToken = dataTypeToken;
			_fileName = fileName;
			_documentIndexDataTypeService = documentIndexDataTypeService;
			_fileService = fileService;
			_versionedFileName = _documentIndexDataTypeService.CreateVersionedFilename(fileName, version);
			_indexFields = indexFields;
			_documentFields = documentFields;
			_version = version;
		}

		public DocumentIndexType CreateVersion(int version)
		{
			return new DocumentIndexType(_typeToken, _dataTypeToken, _fileName, _indexFields, _documentFields, version, _documentIndexDataTypeService, _fileService);
		}

		public ITokensParser CreateTokensParser(DocumentIndexSetup indexSetup)
		{
			return _documentIndexDataTypeService.CreateParser(indexSetup);
		}

		public DocumentIndexTypeToken TypeToken
		{
			get { return _typeToken; }
		}

		public string FileName
		{
			get { return _versionedFileName; }
		}

		public string GetFileFolder(AccountName accountName, DocumentIndexSetup documentIndexSetup)
		{
			string basePath = string.IsNullOrEmpty(documentIndexSetup.IndexPath) ? string.Format("{0}\\{1}", IndexFileRootPath, "index") : documentIndexSetup.IndexPath;
			var isOnSite = accountName == AccountName.Empty;
			return Path.Combine(basePath, isOnSite ? basePath : string.Format("{0}\\{1}", basePath, accountName.Value));
		}

		public IEnumerable<int> GetVersions(AccountName accountName, DocumentIndexSetup documentIndexSetup)
		{
			string folder = GetFileFolder(accountName, documentIndexSetup);
			string versionedFilenameMask = _documentIndexDataTypeService.CreateVersionedFilenameMask(_fileName);
			return _fileService.GetFiles(folder, versionedFilenameMask)
								.Select(name => _documentIndexDataTypeService.ParseVersion(new FileInfo(name).Name))
								.Choose()
								.Distinct()
								.ToList();
		}

		public int Version
		{
			get { return _version; }
		}

		public DocumentIndexDataTypeToken DataTypeToken
		{
			get { return _dataTypeToken; }
		}

		public bool IsBelongedToIndexFields(Enum field)
		{
			return _indexFields.Any(f => f.Equals(field));
		}

		public bool IsBelongedToDocumentFields(Enum field)
		{
			return _documentFields.Any(f => f.Equals(field));
		}

		public IEnumerable<Enum> IndexFields
		{
			get { return _indexFields; }
		}

		public IEnumerable<Enum> DocumentFields
		{
			get { return _documentFields; }
		}

		public override string ToString()
		{
			return "{0} {1}".Fmt(_typeToken, _dataTypeToken);
		}
	}
}
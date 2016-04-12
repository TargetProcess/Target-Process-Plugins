using System;

namespace Tp.Search.Model.Document
{
	class DocumentIndexDataTypeServiceFactory
	{
		public DocumentIndexDataTypeService Create(DocumentIndexDataTypeToken token)
		{
			switch (token)
			{
				case DocumentIndexDataTypeToken.Digits:
					return new DigitsDocumentIndexDataTypeService();
				case DocumentIndexDataTypeToken.Characters:
					return new CharactersDocumentIndexDataTypeService();
				default:
					throw new NotSupportedException(string.Format("{0} is not supported", token));
			}
		}
	}
}
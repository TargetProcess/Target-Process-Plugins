using hOOt;

namespace Tp.Search.Model.Document
{
	class CharactersDocumentIndexDataTypeService : DocumentIndexDataTypeService
	{
		protected override string CreateSuffix()
		{
			return string.Empty;
		}

		public override ITokensParser CreateParser(DocumentIndexSetup indexSetup)
		{
			return new CharacterTokensParser(indexSetup.MinStringLengthToSearch, indexSetup.MaxStringLengthIgnore);
		}
	}
}
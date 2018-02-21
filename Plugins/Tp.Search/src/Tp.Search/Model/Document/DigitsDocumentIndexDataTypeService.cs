using hOOt;

namespace Tp.Search.Model.Document
{
    class DigitsDocumentIndexDataTypeService : DocumentIndexDataTypeService
    {
        protected override string CreateSuffix()
        {
            return "_digits";
        }

        public override ITokensParser CreateParser(DocumentIndexSetup _)
        {
            return new DigitsTokensParser();
        }
    }
}

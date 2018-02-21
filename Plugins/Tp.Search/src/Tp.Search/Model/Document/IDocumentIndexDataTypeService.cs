using System.IO;
using Tp.Core;
using hOOt;

namespace Tp.Search.Model.Document
{
    abstract class DocumentIndexDataTypeService
    {
        private const string VersionMarker = "_v";

        protected abstract string CreateSuffix();
        public abstract ITokensParser CreateParser(DocumentIndexSetup indexSetup);

        public string CreateVersionedFilename(string fileName, int version)
        {
            return fileName + CreateSuffix() + VersionMarker + version;
        }

        public string CreateVersionedFilenameMask(string fileName)
        {
            return fileName + CreateSuffix() + VersionMarker + "*";
        }

        public Maybe<int> ParseVersion(string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            int versionMarker = name.IndexOf(VersionMarker);
            if (versionMarker == -1)
            {
                return Maybe.Nothing;
            }
            string version = name.Substring(versionMarker + VersionMarker.Length);
            int parsedVersion;
            return int.TryParse(version, out parsedVersion)
                ? Maybe.Return(parsedVersion)
                : Maybe.Nothing;
        }
    }
}

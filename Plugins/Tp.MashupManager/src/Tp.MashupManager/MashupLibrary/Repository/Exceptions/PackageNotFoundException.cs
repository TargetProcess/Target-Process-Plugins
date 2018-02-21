using System;

namespace Tp.MashupManager.MashupLibrary.Repository.Exceptions
{
    public class PackageNotFoundException : Exception
    {
        private const string MessageTemplate = "'{0}' package not found";

        public PackageNotFoundException(string packageName) : base(MessageTemplate.Fmt(packageName))
        {
        }
    }
}

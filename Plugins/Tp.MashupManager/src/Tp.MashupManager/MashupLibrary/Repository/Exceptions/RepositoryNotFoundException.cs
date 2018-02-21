using System;

namespace Tp.MashupManager.MashupLibrary.Repository.Exceptions
{
    public class RepositoryNotFoundException : Exception
    {
        private const string MessageTemplate = "'{0}' repository not found";

        public RepositoryNotFoundException(string repositoryName)
            : base(MessageTemplate.Fmt(repositoryName))
        {
        }
    }
}

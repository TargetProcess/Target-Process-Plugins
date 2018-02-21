using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Tp.Git.VersionControlSystem;

namespace Tp.Git
{
    public class LibgitConnectionChecker : IConnectionChecker
    {
        public void Check(GitPluginProfile settings)
        {
            var folder = GitRepositoryFolder.Create(settings.Uri);
            try
            {
                Repository.Clone(settings.Uri, folder.GetAbsolutePath(), new CloneOptions
                {
                    OnProgress = _ => false,
                    CredentialsProvider = LibgitClient.CreateCredentialsHandler(settings)
                });
            }
            catch (UserCancelledException)
            {
                // it's ok, caused by stopping cloning from OnProgress callback above
            }
            finally
            {
                folder.Delete();
            }
        }
    }    
}

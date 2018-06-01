using System;
using NGit.Storage.File;
using NGit.Transport;
using Sharpen;
using StructureMap;
using Tp.Git.VersionControlSystem;
using Tp.Integration.Plugin.Common.Activity;

namespace Tp.Git
{
    public class NGitConnectionChecker : IConnectionChecker
    {
        public void Check(IGitConnectionSettings settings)
        {
            var folder = GitRepositoryFolder.Create(settings.Uri);
            var nativeGit = NGit.Api.Git.Init().SetDirectory(folder.GetAbsolutePath()).Call();
            var transport = Transport.Open(nativeGit.GetRepository(), settings.Uri);
            try
            {
                transport.SetCredentialsProvider(new UsernamePasswordCredentialsProvider(settings.Login, settings.Password));
                transport.OpenFetch();
            }
            catch (EOFException ex)
            {
                throw new InvalidOperationException("Unable to connect to repository. Run 'git fsck' in the repository to check for possible errors.", ex);
            }
            finally
            {
                transport.Close();

                try
                {
                    var git = NGit.Api.Git.Open(folder.GetAbsolutePath());
                    git.GetRepository().Close();
                    WindowCache.Reconfigure(new WindowCacheConfig());
                }
                catch (Exception ex)
                {
                    ObjectFactory.GetInstance<IActivityLogger>().Error(ex);
                }

                folder.Delete();
            }
        }
    }
}

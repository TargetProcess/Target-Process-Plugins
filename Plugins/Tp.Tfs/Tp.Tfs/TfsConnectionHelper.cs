using System;
using System.Linq;
using System.Net;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Tp.SourceControl.Settings;
using WindowsCredential = Microsoft.VisualStudio.Services.Common.WindowsCredential;

namespace Tp.Tfs
{
    public class TfsConnectionParameters
    {
        public Uri TfsCollectionUri { get; set; }
        public string TeamProjectName { get; set; }
        public int SegmentsCount { get; set; }
        public VssCredentials Credential { get; set; }
    }

    public static class TfsConnectionHelper
    {
        public static TfsConnectionParameters GetTfsConnectionParameters(ISourceControlConnectionSettingsSource settings)
        {
            try
            {
                var parameters = new TfsConnectionParameters { Credential = CreateCredential(settings) };

                var uri = new Uri(settings.Uri);
                string[] absolutePathSegments = uri.LocalPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                string possibleVirtualDirectory = absolutePathSegments[0];
                string[] segements = settings.Uri.Split('/');
                string[] serverPathSegments =
                    segements.TakeWhile(x => !x.Equals(possibleVirtualDirectory, StringComparison.OrdinalIgnoreCase)).ToArray();
                string serverPath = string.Join("/", serverPathSegments);

                if (CheckTfsServerPath(serverPath, parameters.Credential))
                {
                    parameters.TfsCollectionUri = new Uri(serverPath + "/" + possibleVirtualDirectory);

                    if (absolutePathSegments.Length > 1)
                        parameters.TeamProjectName = absolutePathSegments[1];

                    parameters.SegmentsCount = absolutePathSegments.Length;

                    return parameters;
                }

                serverPath += "/" + possibleVirtualDirectory;

                if (!CheckTfsServerPath(serverPath, parameters.Credential))
                    throw new TeamFoundationServiceUnavailableException("Could not connect to server.");

                string[] pathSegments =
                    absolutePathSegments.SkipWhile(x => !x.Equals(absolutePathSegments[1], StringComparison.OrdinalIgnoreCase)).ToArray();

                parameters.TfsCollectionUri = new Uri(serverPath + "/" + pathSegments[0]);

                if (pathSegments.Length > 1)
                    parameters.TeamProjectName = pathSegments[1];

                parameters.SegmentsCount = pathSegments.Length;
                return parameters;
            }
            catch (TeamFoundationServiceUnavailableException)
            {
                throw;
            }
            catch
            {
                throw new Exception("Wrong URI format.");
            }
        }

        public static TeamProject[] GetAvailableTeamProjects(ISourceControlConnectionSettingsSource settings)
        {
            var parameters = GetTfsConnectionParameters(settings);

            var collection = new TfsTeamProjectCollection(parameters.TfsCollectionUri, parameters.Credential);
            var vcs = collection.GetService<VersionControlServer>();

            if (string.IsNullOrEmpty(parameters.TeamProjectName))
                return vcs.GetAllTeamProjects(false);

            TeamProject teamProject = vcs.GetTeamProject(parameters.TeamProjectName);
            return new[] { teamProject };
        }

        public static VssCredentials CreateCredential(ISourceControlConnectionSettingsSource settings)
        {
            var domen = string.Empty;
            string login;

            if (settings.Login.IndexOf('\\') > 0)
            {
                domen = settings.Login.Substring(0, settings.Login.IndexOf('\\'));
                login = settings.Login.Substring(settings.Login.IndexOf('\\') + 1);
            }
            else
            {
                login = settings.Login;
            }

            var credential = string.IsNullOrEmpty(domen)
                ? new VssClientCredentials(new VssBasicCredential(new NetworkCredential(login, settings.Password)))
                : new VssClientCredentials(new WindowsCredential(new NetworkCredential(login, settings.Password, domen)));

            return credential;
        }

        public static string[] GetWorkItemTypesForProject(TfsPluginProfile settings)
        {
            if (settings.ProjectsMapping == null || settings.ProjectsMapping.Count == 0)
                return null;

            var parameters = GetTfsConnectionParameters(settings);
            var collection = new TfsTeamProjectCollection(parameters.TfsCollectionUri, parameters.Credential);

            string teamProjectName = settings.ProjectsMapping[0].Key;
            var workItemStore = collection.GetService<WorkItemStore>();
            var workItemTypesCollection = workItemStore.Projects[teamProjectName].WorkItemTypes;
            var workItemTypes = workItemTypesCollection.Cast<WorkItemType>().Select(x => x.Name).ToArray();
            return workItemTypes;
        }

        private static bool CheckTfsServerPath(string path, VssCredentials credential)
        {
            TfsConfigurationServer tfsServer = null;
            try
            {
                tfsServer = new TfsConfigurationServer(new Uri(path), credential);
                tfsServer.Connect(ConnectOptions.None);
                return true;
            }
            catch
            {
                tfsServer?.Dispose();
            }

            return false;
        }
    }
}

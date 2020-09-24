// 
// Copyright (c) 2005-2020 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Settings;
using WindowsCredential = Microsoft.VisualStudio.Services.Common.WindowsCredential;

namespace Tp.Tfs
{
    public enum TfsCollection
    {
        None = 0,
        Project = 1,
        TeamProject = 2
    }

    public class TfsConnectionParameters
    {
        public TfsConnectionParameters(NetworkCredential credential, Uri tfsCollectionUri, string teamProjectName, TfsCollection tfsCollection)
        {
            Credential = credential;
            TfsCollectionUri = tfsCollectionUri;
            TeamProjectName = teamProjectName;
            TfsCollection = tfsCollection;
        }

        public Uri TfsCollectionUri { get; }
        public string TeamProjectName { get; }
        public TfsCollection TfsCollection { get; }
        public NetworkCredential Credential { get; }
    }

    public static class TfsConnectionHelper
    {
        public static TfsConnectionParameters GetTfsConnectionParameters(ISourceControlConnectionSettingsSource settings, out bool useRest, PluginProfileErrorCollection errors = null)
        {
            try
            {
                var uri = new Uri(settings.Uri);
                var credential = CreateCredential(settings);
                var absolutePathSegments = uri.LocalPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var possibleVirtualDirectory = absolutePathSegments[0];
                var segments = settings.Uri.Split('/');
                var serverPathSegments =
                    segments.TakeWhile(x => !x.Equals(possibleVirtualDirectory, StringComparison.OrdinalIgnoreCase)).ToArray();
                var serverPath = string.Join("/", serverPathSegments);

                try
                {
                    if (CheckTfsServerPath(serverPath, credential, out useRest, errors))
                    {
                        return new TfsConnectionParameters(credential, new Uri($"{serverPath}/{possibleVirtualDirectory}"),
                            absolutePathSegments.Length > 1 ? absolutePathSegments[1] : null, (TfsCollection)absolutePathSegments.Length);
                    }
                }
                catch (System.Net.Http.HttpRequestException httpRequestException)
                {
                    if (httpRequestException.InnerException is WebException webException
                        && (webException.Status == WebExceptionStatus.NameResolutionFailure
                            || webException.Status == WebExceptionStatus.TrustFailure
                            || webException.Status == WebExceptionStatus.ConnectFailure))
                    {
                        throw webException; 
                    }
                }
                catch (VssUnauthorizedException)
                {
                    throw;
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                { }

                serverPath = $"{serverPath}/{possibleVirtualDirectory}";

                CheckTfsServerPath(serverPath, credential, out useRest, errors);
                var pathSegments =
                    absolutePathSegments.SkipWhile(x => !x.Equals(absolutePathSegments[1], StringComparison.OrdinalIgnoreCase))
                        .ToArray();

                return new TfsConnectionParameters(credential, new Uri($"{serverPath}/{pathSegments[0]}"),
                    pathSegments.Length > 1 ? pathSegments[1] : null, (TfsCollection)pathSegments.Length);
            }
            catch (VssUnauthorizedException)
            {
                throw;
            }
            catch (VssServiceResponseException)
            {
                throw;
            }
            catch (WebException)
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
            var parameters = GetTfsConnectionParameters(settings, out var _);

            var collection = new TfsTeamProjectCollection(parameters.TfsCollectionUri,
                new VssCredentials(new WindowsCredential(parameters.Credential), CredentialPromptType.DoNotPrompt));
            var vcs = collection.GetService<VersionControlServer>();

            if (string.IsNullOrEmpty(parameters.TeamProjectName))
                return vcs.GetAllTeamProjects(false);

            TeamProject teamProject = vcs.GetTeamProject(parameters.TeamProjectName);
            return new[] { teamProject };
        }

        private static NetworkCredential CreateCredential(ISourceControlConnectionSettingsSource settings)
        {
            var domain = string.Empty;
            string login;

            if (settings.Login.IndexOf('\\') > 0)
            {
                domain = settings.Login.Substring(0, settings.Login.IndexOf('\\'));
                login = settings.Login.Substring(settings.Login.IndexOf('\\') + 1);
            }
            else
            {
                login = settings.Login;
            }

            return string.IsNullOrEmpty(domain)
                ? new NetworkCredential(login, settings.Password)
                : new NetworkCredential(login, settings.Password, domain);
        }

        public static string[] GetWorkItemTypesForProject(TfsPluginProfile settings)
        {
            if (settings.ProjectsMapping == null || settings.ProjectsMapping.Count == 0)
                return null;

            var parameters = GetTfsConnectionParameters(settings, out var _);
            var collection = new TfsTeamProjectCollection(parameters.TfsCollectionUri,
                new VssCredentials(new WindowsCredential(parameters.Credential), CredentialPromptType.DoNotPrompt));

            string teamProjectName = settings.ProjectsMapping[0].Key;
            var workItemStore = collection.GetService<WorkItemStore>();
            var workItemTypesCollection = workItemStore.Projects[teamProjectName].WorkItemTypes;
            var workItemTypes = workItemTypesCollection.Cast<WorkItemType>().Select(x => x.Name).ToArray();
            return workItemTypes;
        }

        private static bool CheckTfsServerPath(string path, NetworkCredential credential, out bool isRest, PluginProfileErrorCollection errors = null)
        {
            try
            {
                using (var vssConnection = new VssConnection(new Uri(path),
                    new VssCredentials(new WindowsCredential(credential), CredentialPromptType.DoNotPrompt),
                    new VssClientHttpRequestSettings
                    {
                        ServerCertificateValidationCallback =
                            delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                            {
                                if (!(sender is HttpWebRequest))
                                {
                                    //logging
                                    return true;
                                }

                                // If the certificate is a valid, signed certificate, return true.
                                if (sslPolicyErrors == SslPolicyErrors.None)
                                {
                                    return true;
                                }

                                // If there are errors in the certificate chain, look at each error to determine the cause.
                                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
                                {
                                    if (chain != null)
                                    {
                                        foreach (X509ChainStatus status in chain.ChainStatus)
                                        {
                                            if ((certificate.Subject == certificate.Issuer) &&
                                                (status.Status == X509ChainStatusFlags.UntrustedRoot))
                                            {
                                                errors?.Add(new PluginProfileError
                                                {
                                                    Status = PluginProfileErrorStatus.TrustFailure,
                                                    FieldName = "Uri",
                                                    Message = status.StatusInformation
                                                });
                                                // Self-signed certificates with an untrusted root are valid.
                                                continue;
                                            }

                                            if (status.Status != X509ChainStatusFlags.NoError)
                                            {
                                                errors?.Add(new PluginProfileError
                                                {
                                                    Status = PluginProfileErrorStatus.Error,
                                                    FieldName = "Uri",
                                                    Message = status.StatusInformation
                                                });
                                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                                // so the method returns false.
                                                return false;
                                            }
                                        }
                                    }

                                    // When processing reaches this line, the only errors in the certificate chain are
                                    // untrusted root errors for self-signed certificates. These certificates are valid
                                    // for default Exchange Server installations, so return true.
                                    return true;
                                }

                                // In all other cases, return false.
                                return false;
                            }
                    })
                )
                {
                    isRest = true;
                    vssConnection.ConnectAsync().SyncResult();
                    return vssConnection.HasAuthenticated;
                }
            }
            catch (VssServiceResponseException vssServiceResponseException)
            {
                if (vssServiceResponseException.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    var tfsServer = new TfsConfigurationServer(new Uri(path),
                        new VssCredentials(new WindowsCredential(credential), CredentialPromptType.DoNotPrompt));
                    tfsServer.Connect(Microsoft.TeamFoundation.Framework.Common.ConnectOptions.None);

                    isRest = false;
                    return tfsServer.HasAuthenticated;
                }
                throw;
            }
        }
    }
}

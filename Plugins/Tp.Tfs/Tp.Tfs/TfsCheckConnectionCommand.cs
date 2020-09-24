// 
// Copyright (c) 2005-2019 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Linq;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.VersionControlSystem;
using Tp.Tfs.VersionControlSystem;

namespace Tp.Tfs
{
    public class TfsCheckConnectionCommand : VcsCheckConnectionCommand<TfsPluginProfile>
    {
        public TfsCheckConnectionCommand(IProfileCollection profileCollection) : base(profileCollection)
        {
        }

        protected override void CheckStartRevision(
            TfsPluginProfile settings,
            IVersionControlSystem versionControlSystem,
            PluginProfileErrorCollection errors)
        {
            settings.ValidateStartRevision(errors);
        }

        protected override void OnCheckConnection(PluginProfileErrorCollection errors, TfsPluginProfile settings)
        {
            settings.ValidateStartRevision(errors);
            settings.ValidateUri(errors);

            if (!errors.Any())
            {
                try
                {
                    var parameters = TfsConnectionHelper.GetTfsConnectionParameters(settings, out var useRest);
                    using (var tfsClient = useRest
                        ? (ITfsClient)new TfvcHttpTfsClient(parameters, TimeSpan.FromSeconds(100))
                        : new TfsClient(parameters, TimeSpan.FromSeconds(100)))
                    {
                        var latest = tfsClient.GetLatestChangesetId(int.Parse(settings.StartRevision));
                        if (latest == null)
                        {
                            errors.Add(new PluginProfileError
                            {
                                FieldName = "StartRevision",
                                Message = "No revisions found.",
                                Status = PluginProfileErrorStatus.UnexistedRevisionWarning
                            });
                        }
                        else if (latest.Value < int.Parse(settings.StartRevision))
                        {
                            errors.Add(new PluginProfileError
                            {
                                FieldName = "StartRevision",
                                Message = $"The last revision {latest.Value} is less than the start revision {settings.StartRevision}",
                                Status = PluginProfileErrorStatus.UnexistedRevisionWarning
                            });
                        }
                    }
                    /*switch (parameters.SegmentsCount)
                    {
                        case UriTfsProjectCollection:
                        {
                            vssConnection = new VssConnection(parameters.TfsCollectionUri,
                                new VssCredentials(parameters.Credential) { PromptType = CredentialPromptType.DoNotPrompt });
                            vssConnection.ConnectAsync().SyncResult();

                            var vcs = vssConnection.GetClient<TfvcHttpClient>();
                            CheckChangeset(settings, vcs);

                            break;
                        }
                        case UriTfsTeamProject:
                        {
                            vssConnection = new VssConnection(parameters.TfsCollectionUri,
                                new VssCredentials(parameters.Credential) { PromptType = CredentialPromptType.DoNotPrompt });
                            vssConnection.ConnectAsync().SyncResult();

                            try
                            {
                                var pc = vssConnection.GetClient<ProjectHttpClient>();
                                var teamProject = pc.GetProject(parameters.TeamProjectName).Result;

                                var vcs = vssConnection.GetClient<TfvcHttpClient>();
                                CheckChangeset(settings, vcs, teamProject);
                            }

                            catch (AggregateException aggregateException)
                            {
                                foreach (var exception in aggregateException.InnerExceptions)
                                {
                                    if (exception is ProjectDoesNotExistWithNameException vssServiceException)
                                    {
                                        errors.Add(new PluginProfileError
                                        {
                                            FieldName = "Uri",
                                            Message = vssServiceException.Message
                                        });
                                    }
                                }
                            }*/
                            /*catch (Microsoft.TeamFoundation.VersionControl.Client.VersionControlException)
                            {
                                var gitRepositoryService = collection.GetService<GitRepositoryService>();
                                var gitRepositories = gitRepositoryService.QueryRepositories(parameters.TeamProjectName);
                                var gitRepository = gitRepositories.Single(gr => gr.Name.Equals(parameters.TeamProjectName));
                                if (gitRepository != null)
                                {
                                    errors.Add(new PluginProfileError
                                    {
                                        FieldName = "Uri",
                                        Message =
                                            $"Git team project is not supported, use Git plugin with '{gitRepository.RemoteUrl}' instead."
                                    });
                                }
                                else
                                {
                                    throw;
                                }
                            }*/

                            /*break;
                        }
                        default:
                            errors.Add(new PluginProfileError { FieldName = "Uri", Message = "Could not connect to server." });
                            break;
                    }*/
                }
                catch (VssUnauthorizedException e)
                {
                    errors.Add(
                        new PluginProfileError
                        {
                            Status = PluginProfileErrorStatus.WrongCredentialsError,
                            FieldName = "Login",
                            Message = e.Message,
                            AdditionalInfo = e.Message
                        });
                    errors.Add(
                        new PluginProfileError
                        {
                            Status = PluginProfileErrorStatus.WrongCredentialsError,
                            FieldName = "Password",
                            Message = e.Message,
                            AdditionalInfo = e.Message
                        });
                }
                catch (VssServiceResponseException e)
                {
                    errors.Add(new PluginProfileError
                    {
                        AdditionalInfo = e.Message,
                        FieldName = "Uri",
                        Message = $"{e.Message} HttpStatusCode: {e.HttpStatusCode}",
                        Status = PluginProfileErrorStatus.Error
                    });
                }
                catch (ChangesetNotFoundException e)
                {
                    errors.Add(new PluginProfileError
                    {
                        AdditionalInfo = e.Message,
                        FieldName = "StartRevision",
                        Message = "There’s no such revision.",
                        Status = PluginProfileErrorStatus.UnexistedRevisionWarning
                    });
                }
                catch (TeamFoundationServiceUnavailableException e)
                {
                    errors.Add(new PluginProfileError
                    {
                        AdditionalInfo = e.Message,
                        FieldName = "Uri",
                        Message = "Could not connect to server.",
                    });
                }
                /*finally
                {
                    vssConnection?.Dispose();
                }*/
            }
        }

        /*private static void CheckChangeset(TfsPluginProfile settings, TfvcHttpClientBase versionControl, TeamProjectReference teamProject = null)
        {
            var startRevision = int.Parse(settings.StartRevision) > 1 ? int.Parse(settings.StartRevision) - 1: int.Parse(settings.StartRevision);

            var tfvcChangesetSearchCriteria = new TfvcChangesetSearchCriteria { ItemPath = "$/", FromId = startRevision };

            try
            {
                var changeset = teamProject == null
                    ? versionControl.GetChangesetsAsync(null, null, 0, 1, "Id ASC", tfvcChangesetSearchCriteria).Result.FirstOrDefault()
                    : versionControl.GetChangesetsAsync(teamProject.Id, null, 0, 1, "Id ASC", tfvcChangesetSearchCriteria).Result
                        .FirstOrDefault();

                if (changeset != null)
                    return;
            }
            catch (AggregateException aggregateException)
            {
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    if (exception is VssServiceException vssServiceException && vssServiceException.Message.Contains("TF14019"))
                    {
                        throw new ChangesetNotFoundException(vssServiceException.Message);
                    }
                }
                throw aggregateException.Flatten();
            }
        }*/
    }
}

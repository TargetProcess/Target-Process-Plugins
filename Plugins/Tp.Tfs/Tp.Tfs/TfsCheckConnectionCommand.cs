// 
// Copyright (c) 2005-2012 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Globalization;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using Tp.Integration.Plugin.Common.Validation;
using Tp.SourceControl.Commands;
using Tp.SourceControl.VersionControlSystem;
using System.Linq;

namespace Tp.Tfs
{
	public class TfsCheckConnectionCommand : VcsCheckConnectionCommand<TfsPluginProfile>
	{
		private const int UriTfsProjectCollection = 1;
		private const int UriTfsTeamProject = 2;

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
				TfsTeamProjectCollection collection = null;

				try
				{
					TfsConnectionParameters parameters = TfsConnectionHelper.GetTfsConnectionParameters(settings);

					switch (parameters.SegmentsCount)
					{
						case UriTfsProjectCollection:
							{
								collection = new TfsTeamProjectCollection(parameters.TfsCollectionUri, parameters.Credential);
								collection.EnsureAuthenticated();
								collection.Connect(ConnectOptions.None);

								var vcs = collection.GetService<VersionControlServer>();
								CheckChangeset(settings, vcs);

								break;
							}
						case UriTfsTeamProject:
							{
								collection = new TfsTeamProjectCollection(parameters.TfsCollectionUri, parameters.Credential);
								collection.EnsureAuthenticated();
								collection.Connect(ConnectOptions.None);

								var vcs = collection.GetService<VersionControlServer>();
								TeamProject teamProject = vcs.GetTeamProject(parameters.TeamProjectName);

								CheckChangeset(settings, vcs, teamProject);

								break;
							}
						default:
							errors.Add(new PluginProfileError { FieldName = "Uri", Message = "Could not connect to server." });
							break;
					}
				}
				catch (TeamFoundationServerUnauthorizedException e)
				{
					errors.Add(
							new PluginProfileError
								{
								Status = PluginProfileErrorStatus.WrongCredentialsError,
								FieldName = "Login",
								Message = "Authorization failed.",
								AdditionalInfo = e.Message
							});
					errors.Add(
							new PluginProfileError
								{
								Status = PluginProfileErrorStatus.WrongCredentialsError,
								FieldName = "Password",
								Message = "Authorization failed.",
								AdditionalInfo = e.Message
							});
				}
				catch (ResourceAccessException e)
				{
					errors.Add(new PluginProfileError
						{
						AdditionalInfo = e.Message,
						FieldName = "Uri",
						Message = "Resource access denied",
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
				finally
				{
					if (collection != null)
						collection.Dispose();
				}
			}
		}

		private static void CheckChangeset(TfsPluginProfile settings, VersionControlServer versionControl, TeamProject teamProject = null)
		{
			string startRevision = int.Parse(settings.StartRevision) > 1 ?
					(int.Parse(settings.StartRevision) - 1).ToString(CultureInfo.InvariantCulture) :
					settings.StartRevision;

			if (teamProject == null)
			{
				var teamProjects = versionControl.GetAllTeamProjects(false);

				foreach (var project in teamProjects)
				{
					try
					{
						var changeset = versionControl.QueryHistory(
								project.ServerItem,
								VersionSpec.Latest,
								0,
								RecursionType.Full,
								null,
								VersionSpec.ParseSingleSpec(startRevision, null),
								null,
								1,
								false,
								false).Cast<Changeset>().FirstOrDefault();

						if (changeset != null)
							return;
					}
					catch (ChangesetNotFoundException) { }
				}

				throw new ChangesetNotFoundException($"There’s no such revision: {settings.StartRevision}");
			}

			var projectChangeset = versionControl.QueryHistory(
					teamProject.ServerItem,
					VersionSpec.Latest,
					0,
					RecursionType.Full,
					null,
					VersionSpec.ParseSingleSpec(startRevision, null),
					null,
					1,
					false,
					false).Cast<Changeset>().FirstOrDefault();

			if (projectChangeset == null)
				throw new ChangesetNotFoundException($"There’s no such revision: {settings.StartRevision}");
		}
	}
}
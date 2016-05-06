// 
// Copyright (c) 2005-2016 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using StructureMap;
using Tp.BugTracking.Settings;
using Tp.Bugzilla.BugzillaQueries;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;
using Tp.Integration.Plugin.Common.Validation;

namespace Tp.Bugzilla
{
	public class BugzillaUrl
	{
		private readonly IBugTrackingConnectionSettingsSource _profile;
		private readonly IActivityLogger _log;

		public BugzillaUrl(IBugTrackingConnectionSettingsSource profile)
			: this(profile, ObjectFactory.GetInstance<IActivityLogger>())
		{
		}

		public BugzillaUrl(IStorageRepository storageRepository, IActivityLogger logger)
			: this(storageRepository.GetProfile<BugzillaProfile>(), logger)
		{
		}

		private BugzillaUrl(IBugTrackingConnectionSettingsSource profile, IActivityLogger logger)
		{
			_log = logger;
			_profile = profile;
		}

		public string Url => string.IsNullOrEmpty(_profile.Url) ? string.Empty : _profile.Url.TrimEnd('/', '\\');

		public string CheckConnection()
		{
			return UploadDataToBugzilla("cmd=check");
		}

		public bugCollection GetBugs(int[] bugIDs)
		{
			var content = UploadDataToBugzilla($"cmd=get_bugs&id={bugIDs.ToString(",")}");

			content = ProcessResponse(content);

			_log.Debug("Parsing content");

			if (string.IsNullOrEmpty(content))
				return new bugCollection();

			var parser = new BugzillaParser<bugzilla>();
			var bugzilla = parser.Parse(content);

			return bugzilla.bugCollection;
		}

		public int[] GetChangedBugsIds(DateTime? date)
		{
			var myDtfi = new CultureInfo("en-US", false).DateTimeFormat;
			var url =
				$"cmd=get_bug_ids&name={HttpUtility.UrlEncode(_profile.SavedSearches)}&date={string.Format(myDtfi, "{0:yyyy-MM-dd HH:mm:ss}", date)}";

			var content = UploadDataToBugzilla(url);

			content = ProcessResponse(content);

			try
			{
				var ids = content.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

				return ids.Select(int.Parse).ToArray();
			}
			catch (Exception)
			{
				throw new InvalidOperationException(
					$"Unable to take bug ids from Bugzilla. Bugzilla returned an error: {content}");
			}
		}

		public string GetBugExternalUrl(string bugId)
		{
			return $"{Url}/show_bug.cgi?id={bugId}";
		}

		private string UploadDataToBugzilla(string query)
		{
			var encoding = Encoding.UTF8;
			var webClient = new TpWebClient(new PluginProfileErrorCollection()) { Encoding = encoding };
			webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

			if (!string.IsNullOrEmpty(_profile.Login) && !string.IsNullOrEmpty(_profile.Password))
			{
				query += $"&Bugzilla_login={_profile.Login}&Bugzilla_password={_profile.Password}";
			}

			var bret = webClient.UploadData($"{Url}/tp2.cgi", "POST", encoding.GetBytes(query));

			var content = encoding.GetString(bret);

			return content;
		}

		public string ExecuteOnBugzilla(IBugzillaQuery bugzillaQuery)
		{
			var content = UploadDataToBugzilla(bugzillaQuery.Value());

			return ProcessResponse(content);
		}

		private static string ProcessResponse(string content)
		{
			return new BugzillaResponseProcessor(content).Process();
		}
	}
}
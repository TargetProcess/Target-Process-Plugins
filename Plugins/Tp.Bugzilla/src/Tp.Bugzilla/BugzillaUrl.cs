// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
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
using Tp.Bugzilla.ConnectionValidators;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

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

		public string Url
		{
			get { return String.IsNullOrEmpty(_profile.Url) ? String.Empty : _profile.Url.TrimEnd('/', '\\'); }
		}

		public string CheckConnection()
		{
			return UploadDataToBugzilla("cmd=check");
		}

		public bugCollection GetBugs(int[] bugIDs)
		{
			var content = UploadDataToBugzilla(String.Format("cmd=get_bugs&id={0}", bugIDs.ToString(",")));

			content = ParseResponse(content);

			_log.Debug("Parsing content");

			if (string.IsNullOrEmpty(content))
				return new bugCollection();

			var parser = new BugzillaParser<bugzilla>();
			var bugzilla = parser.Parse(content);

			return bugzilla.bugCollection;
		}

		public int[] GetChangedBugsIds(IFormattable date)
		{
			var myDtfi = new CultureInfo("en-US", false).DateTimeFormat;
			var url = String.Format("cmd=get_bug_ids&name={0}&date={1}",
			                        HttpUtility.UrlEncode(_profile.SavedSearches),
			                        date.ToString("dd-MMM-yyyy HH:mm:ss", myDtfi));

			var content = UploadDataToBugzilla(url);

			content = ParseResponse(content);

			try
			{
				var ids = content.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

				return ids.Select(Int32.Parse).ToArray();
			}
			catch (Exception)
			{
				throw new InvalidOperationException(string.Format("Unable to take bug ids from Bugzilla. Bugzilla returned an error: {0}", content));
			}
		}

		public string GetBugExternalUrl(string bugId)
		{
			return string.Format("{0}/show_bug.cgi?id={1}", Url, bugId);
		}

		private string UploadDataToBugzilla(string query)
		{
			var webClient = new WebClient {Encoding = Encoding.UTF8};
			webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

			if (!String.IsNullOrEmpty(_profile.Login) && !String.IsNullOrEmpty(_profile.Password))
			{
				query += String.Format("&Bugzilla_login={0}&Bugzilla_password={1}", _profile.Login, _profile.Password);
			}

			var bret = webClient.UploadData(String.Format("{0}/tp2.cgi", Url), "POST", Encoding.ASCII.GetBytes(query));

			var content = Encoding.ASCII.GetString(bret);

			return content;
		}

		public string ExecuteOnBugzilla(IBugzillaQuery bugzillaQuery)
		{
			var content = UploadDataToBugzilla(bugzillaQuery.Value());

			content = ParseResponse(content);

			return content;
		}

		private static string ParseResponse(string content)
		{
			var response = CheckVersions(content);

			AssertIsNotError(response);

			return response;
		}

		private static string CheckVersions(string content)
		{
			var parts = content.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

			if (parts.Length < 3)
			{
				throw new ApplicationException("tp2.cgi is obsolete. Please update tp2.cgi and try again.");
			}

			const string BUGZILLA_VERSION = "BUGZILLA VERSION: ";
			var bugzillaVersion = GetVersionFromResponse(parts[0], BUGZILLA_VERSION);

			if (!SettingsValidator.BugzillaVersionIsSupported(bugzillaVersion))
			{
				throw new ApplicationException(string.Format(SettingsValidator.BUGZILLA_VERSION_IS_NOT_SUPPORTED_BY_PLUGIN, bugzillaVersion));
			}

			const string SUPPORTED_BUGZILLA_VERSION = "SUPPORTED BUGZILLA VERSION: ";
			var supportedBugzillaVersion = GetVersionFromResponse(parts[1], SUPPORTED_BUGZILLA_VERSION);

			if (!SettingsValidator.ScriptSupportsProvidedBugzillaVersion(bugzillaVersion, supportedBugzillaVersion))
			{
				throw new ApplicationException(string.Format(SettingsValidator.BUGZILLA_VERSION_IS_NOT_SUPPORTED_BY_TP2_CGI, bugzillaVersion));
			}

			const string SCRIPT_VERSION = "SCRIPT VERSION: ";
			var scriptVersion = GetVersionFromResponse(parts[2], SCRIPT_VERSION);

			if (!SettingsValidator.ScriptVersionIsValid(scriptVersion))
			{
				throw new ApplicationException(string.Format(SettingsValidator.TP2_CGI_IS_NOT_SUPPORTED_BY_THIS_PLUGIN, scriptVersion));
			}

			return string.Join("\n", parts.Skip(3));
		}

		private static string GetVersionFromResponse(string source, string key)
		{
			if (!source.StartsWith(key))
			{
				throw new ApplicationException("tp2.cgi is obsolete. Please update tp2.cgi and try again.");
			}

			return source.Substring(key.Length);
		}

		private static void AssertIsNotError(string content)
		{
			if (content.StartsWith("ERROR"))
			{
				throw new ApplicationException(content.Substring(7));
			}
		}
	}
}
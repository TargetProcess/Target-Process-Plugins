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
using Tp.Bugzilla.BugzillaQueries;
using Tp.Bugzilla.Schemas;
using Tp.Integration.Plugin.Common.Activity;
using Tp.Integration.Plugin.Common.Domain;

namespace Tp.Bugzilla
{
	public class BugzillaUrl
	{
		private readonly BugzillaProfile _profile;
		private readonly IActivityLogger _log;

		public BugzillaUrl(BugzillaProfile profile) : this(profile, ObjectFactory.GetInstance<IActivityLogger>())
		{
		}

		public BugzillaUrl(IStorageRepository storageRepository, IActivityLogger logger)
			: this(storageRepository.GetProfile<BugzillaProfile>(), logger)
		{
		}

		private BugzillaUrl(BugzillaProfile profile, IActivityLogger logger)
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
			                        HttpUtility.UrlEncode(_profile.Queries),
			                        date.ToString("dd-MMM-yyyy HH:mm", myDtfi));

			var content = UploadDataToBugzilla(url);
			var ids = content.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

			return ids.Select(Int32.Parse).ToArray();
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
			return UploadDataToBugzilla(bugzillaQuery.Value());
		}
	}
}
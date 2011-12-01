// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Tp.Bugzilla.BugzillaQueries
{
	public class BugzillaCommentAction : IBugzillaAction
	{
		private readonly string _bugzillaBugId;
		private readonly string _commentText;
		private readonly string _owner;
		private readonly DateTime _createDate;

		public BugzillaCommentAction(string bugzillaBugId, string commentText, string owner, DateTime createDate)
		{
			_bugzillaBugId = bugzillaBugId;
			_commentText = commentText;
			_owner = owner;
			_createDate = createDate;
		}

		public string Value()
		{
			var decoded = HttpUtility.HtmlDecode(_commentText);
			decoded = decoded.Replace("<br>", "\r\n");
			decoded = decoded.Replace("<br/>", "\r\n");
			decoded = decoded.Replace("<br />", "\r\n");
			//remove &nbsp;
			decoded = decoded.Replace(((char) 160).ToString(), " ");
			var text = Regex.Replace(decoded, "<[^>]*>", string.Empty);

			var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(text));

			return string.Format("cmd=add_comment&bugid={0}&comment_text={1}&owner={2}&date={3}", _bugzillaBugId,
			                     base64String,
			                     _owner, _createDate.ToString("u"));
		}

		public string GetOperationDescription()
		{
			return string.Format("Add comment to bug with id '{0}' and owner '{1}'", _bugzillaBugId, _owner);
		}
	}
}
// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using StructureMap;

namespace Tp.Bugzilla.BugzillaQueries
{
    public class BugzillaChangeStatusAction : IBugzillaAction
    {
        private readonly string _bugzillaBugId;
        private readonly string _status;
        private readonly string _commentOnChangingState;
        private readonly IBugzillaService _bugzillaService;
        private readonly IBugzillaInfoStorageRepository _bugzillaInfoStorageRepository;

        public BugzillaChangeStatusAction(string bugzillaBugId, string status, string commentOnChangingState)
        {
            _bugzillaBugId = bugzillaBugId;
            _status = status;
            _commentOnChangingState = commentOnChangingState;
            _bugzillaService = ObjectFactory.GetInstance<IBugzillaService>();
            _bugzillaInfoStorageRepository = ObjectFactory.GetInstance<IBugzillaInfoStorageRepository>();
        }

        public string Value()
        {
            var resolution = GetResolution();
            var duplicateBugId = GetDuplicateBugId(resolution);

            return string.Format("cmd=change_status&id={0}&status={1}{2}{3}",
                _bugzillaBugId, _status,
                string.IsNullOrEmpty(resolution) ? string.Empty : "&resolution=" + resolution,
                string.IsNullOrEmpty(duplicateBugId) ? string.Empty : "&dup_id=" + duplicateBugId);
        }

        public string GetOperationDescription()
        {
            return $"Change status for bug with id '{_bugzillaBugId}' to status '{_status}'";
        }

        private string GetDuplicateBugId(string resolution)
        {
            const string duplicate = "DUPLICATE";

            if (!duplicate.Equals(resolution, StringComparison.InvariantCultureIgnoreCase))
                return string.Empty;

            var regex = new Regex("(D|d)(U|u)(P|p)(L|l)(I|i)(C|c)(A|a)(T|t)(E|e):\\s*#?(?<id>[0-9]+)",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var matches = regex.Matches(HttpUtility.HtmlDecode(_commentOnChangingState.ToLower()));

            int result;
            if (matches.Count <= 0 || !Int32.TryParse(matches[0].Groups["id"].Value, out result))
                return string.Empty;

            var bugzillaBug = _bugzillaInfoStorageRepository.GetBugzillaBug(result);

            return bugzillaBug != null ? bugzillaBug.Id : string.Empty;
        }

        private string GetResolution()
        {
            if (string.IsNullOrEmpty(_commentOnChangingState))
            {
                return string.Empty;
            }

            return _bugzillaService.GetResolutions()
                .SingleOrDefault(
                    resolution =>
                        _commentOnChangingState.IndexOf(resolution, StringComparison.InvariantCultureIgnoreCase) != -1
                        && !string.IsNullOrEmpty(resolution));
        }
    }
}

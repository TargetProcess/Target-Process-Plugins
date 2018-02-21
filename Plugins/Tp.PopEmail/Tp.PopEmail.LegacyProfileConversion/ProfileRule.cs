// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Text;

namespace Tp.PopEmailIntegration.LegacyProfileConversion
{
    public class ProfileRule
    {
        private readonly bool _createRequest;
        private string _subjectContainsCondition;
        private readonly string _projectId;

        public ProfileRule(string projectId)
        {
            _projectId = projectId;
        }

        public ProfileRule(string projectId, bool createRequest)
        {
            _projectId = projectId;
            _createRequest = createRequest;
        }

        public string ProjectId
        {
            get { return _projectId; }
        }

        public void AddSubjectContainsCondition(string value)
        {
            if (!string.IsNullOrEmpty(_subjectContainsCondition))
            {
                _subjectContainsCondition += ",";
            }

            _subjectContainsCondition += value;
        }

        public override string ToString()
        {
            var res = new StringBuilder();
            if (!string.IsNullOrEmpty(_subjectContainsCondition))
            {
                res.AppendFormat("when subject contains '{0}' ", _subjectContainsCondition);
            }

            res.AppendFormat("then attach to project {0}", _projectId);

            if (_createRequest)
            {
                res.AppendFormat(" and create request in project {0}", _projectId);
            }

            return res.ToString();
        }
    }
}

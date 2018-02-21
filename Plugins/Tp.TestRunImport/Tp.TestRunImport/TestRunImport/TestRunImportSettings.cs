// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Runtime.Serialization;

namespace Tp.Integration.Plugin.TestRunImport.TestRunImport
{
    [DataContract]
    public class TestRunImportSettings
    {
        [DataMember]
        public string ResultsFilePath { get; set; }

        [DataMember]
        public bool PassiveMode { get; set; }

        [DataMember]
        public int Project { get; set; }

        [DataMember]
        public int TestPlan { get; set; }

        [DataMember]
        public string RegExp { get; set; }

        [DataMember]
        public int AuthTokenUserId { get; set; }

        [DataMember]
        public string RemoteResultsUrl { get; set; }

        [DataMember]
        public bool PostResultsToRemoteUrl { get; set; }

        [DataMember]
        public FrameworkTypes.FrameworkTypes FrameworkType { get; set; }
    }
}

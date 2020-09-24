﻿using System;
using System.Runtime.Serialization;

namespace Tp.Integration.Messages.PluginLifecycle
{
    [Serializable, DataContract]
    public class ProfileErrorCheckResult
    {
        [DataMember]
        public string ProfileName { get; set; }

        [DataMember]
        public bool ErrorsExist { get; set; }

        [DataMember]
        public DateTime? ErrorDate { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}

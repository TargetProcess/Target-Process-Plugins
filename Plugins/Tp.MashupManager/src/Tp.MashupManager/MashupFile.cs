// 
// Copyright (c) 2005-2013 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Runtime.Serialization;

namespace Tp.MashupManager
{
    [DataContract, Serializable]
    public class MashupFile
    {
        /// <summary>
        /// File name without path
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string Content { get; set; }
    }
}

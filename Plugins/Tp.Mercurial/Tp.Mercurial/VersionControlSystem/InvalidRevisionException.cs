// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;

namespace Tp.Mercurial.VersionControlSystem
{
    public class InvalidRevisionException : Exception
    {
        private static readonly string Msg =
            $"should be between {MercurialRevisionId.UtcTimeMin.ToShortDateString()} and {MercurialRevisionId.UtcTimeMax.ToShortDateString()}";

        public override string Message => Msg;
    }
}

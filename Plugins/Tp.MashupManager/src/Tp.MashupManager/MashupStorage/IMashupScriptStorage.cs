// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages;

namespace Tp.MashupManager.MashupStorage
{
    public interface IMashupScriptStorage
    {
        Mashup GetMashup(string mashupName);
        Mashup GetMashup(AccountName account, string mashupName);
        void SaveMashup(Mashup mashup);
        void DeleteMashup(string mashup);
    }
}

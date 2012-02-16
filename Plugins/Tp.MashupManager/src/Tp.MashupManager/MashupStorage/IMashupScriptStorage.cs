// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using Tp.Integration.Messages;
using Tp.MashupManager.Dtos;

namespace Tp.MashupManager.MashupStorage
{
	public interface IMashupScriptStorage
	{
		MashupDto GetMashup(string mashupName);
		MashupDto GetMashup(AccountName account, string mashupName);
		void SaveMashup(MashupDto mashup);
		void DeleteMashup(string mashup);
	}
}
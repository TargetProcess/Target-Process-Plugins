// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common;
using Tp.MashupManager.MashupLibrary.Repository.Config;

namespace Tp.MashupManager
{
	[Serializable, Profile, DataContract]
	public class MashupManagerProfile
	{
		public MashupManagerProfile()
		{
			MashupNames = new MashupNames();
		}

		[DataMember]
		public MashupNames MashupNames { get; set; }

		[DataMember]
		public LibraryRepositoryConfig[] LibraryRepositoryConfigs { get; set; }
	}

	public class MashupNames : List<string>
	{
	}
}
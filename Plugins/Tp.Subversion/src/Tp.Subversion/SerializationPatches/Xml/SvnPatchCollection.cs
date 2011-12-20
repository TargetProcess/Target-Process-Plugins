// 
// Copyright (c) 2005-2011 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 

using System.Collections;
using System.Collections.Generic;
using Tp.Integration.Messages.SerializationPatches;

namespace Tp.Subversion.SerializationPatches.Xml
{
	internal class SvnPatchCollection : IPatchCollection
	{
		public IEnumerator<IPatch> GetEnumerator()
		{
			return
				new List<IPatch>
					{
						new NewRevisionRangeDetectedLocalMessagePatch(),
						new RevisionRangeInStoragePatch(),
						new NewRevisionDetectedLocalMessagePatch(),
						new RevisionCreatedLocalMessagePatch(),
						new SvnActionPatch(),
						new CreateRevisionSagaPatch(),
						new AttachToEntitySagaPatch()
					}.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
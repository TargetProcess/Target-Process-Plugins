// 
// Copyright (c) 2005-2010 TargetProcess. All rights reserved.
// TargetProcess proprietary/confidential. Use is subject to license terms. Redistribution of this file is strictly forbidden.
// 
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tp.Integration.Messages.ComponentModel
{
	[Serializable]
	public class SerializableStringDictionary : SerializableDictionary<string, string>
	{
		public SerializableStringDictionary()
		{
		}

		public SerializableStringDictionary(IDictionary<string, string> dictionary) : base(dictionary)
		{
		}

		public SerializableStringDictionary(IEqualityComparer<string> comparer) : base(comparer)
		{
		}

		public SerializableStringDictionary(int capacity) : base(capacity)
		{
		}

		public SerializableStringDictionary(IDictionary<string, string> dictionary, IEqualityComparer<string> comparer)
			: base(dictionary, comparer)
		{
		}

		public SerializableStringDictionary(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer)
		{
		}

		protected SerializableStringDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}